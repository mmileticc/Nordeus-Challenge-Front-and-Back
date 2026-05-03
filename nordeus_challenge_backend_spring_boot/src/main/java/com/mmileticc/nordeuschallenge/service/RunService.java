package com.mmileticc.nordeuschallenge.service;

import com.mmileticc.nordeuschallenge.exeption.BadRequestException;
import com.mmileticc.nordeuschallenge.exeption.ResourceNotFoundException;
import com.mmileticc.nordeuschallenge.model.dto.BattleSessionCreateRequestDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleSessionDTO;
import com.mmileticc.nordeuschallenge.model.dto.MoveDTO;
import com.mmileticc.nordeuschallenge.model.dto.RunConfigDTO;
import com.mmileticc.nordeuschallenge.model.dto.RunEncounterDTO;
import com.mmileticc.nordeuschallenge.model.dto.RunHeroProgressDTO;
import com.mmileticc.nordeuschallenge.model.dto.RunResolveEncounterResponseDTO;
import com.mmileticc.nordeuschallenge.model.dto.RunStartEncounterResponseDTO;
import com.mmileticc.nordeuschallenge.model.entity.Hero;
import com.mmileticc.nordeuschallenge.model.entity.Monster;
import com.mmileticc.nordeuschallenge.model.entity.Move;
import com.mmileticc.nordeuschallenge.repository.HeroRepository;
import com.mmileticc.nordeuschallenge.repository.MonsterRepository;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.HashSet;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Optional;
import java.util.Set;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;
import java.util.concurrent.ThreadLocalRandom;
import java.util.stream.Collectors;

@Service
public class RunService {

    private static final int MAX_EQUIPPED_MOVES = 4;

    private final HeroRepository heroRepository;
    private final MonsterRepository monsterRepository;
    private final BattleSessionService battleSessionService;
    private final ConcurrentMap<UUID, RunContext> runs = new ConcurrentHashMap<>();

    public RunService(
            HeroRepository heroRepository,
            MonsterRepository monsterRepository,
            BattleSessionService battleSessionService
    ) {
        this.heroRepository = heroRepository;
        this.monsterRepository = monsterRepository;
        this.battleSessionService = battleSessionService;
    }

    public RunConfigDTO startRun(Long requestedHeroId) {
        long heroId = requestedHeroId == null ? 1L : requestedHeroId;
        Hero hero = heroRepository.findById(heroId)
                .orElseThrow(() -> new ResourceNotFoundException("Hero not found: " + heroId));

        List<Monster> encounters = monsterRepository.findAll().stream()
                .sorted(Comparator.comparingInt(Monster::getDifficultyRank))
                .limit(5)
                .toList();

        if (encounters.size() < 5) {
            throw new BadRequestException("At least 5 monsters are required to start a run");
        }

        LinkedHashMap<Long, Move> learnedMoves = hero.getMoves().stream()
                .sorted(Comparator.comparingLong(Move::getId))
                .collect(Collectors.toMap(Move::getId, move -> move, (a, b) -> a, LinkedHashMap::new));

        List<Long> equippedMoveIds = learnedMoves.keySet().stream()
                .limit(MAX_EQUIPPED_MOVES)
                .toList();

        RunContext context = new RunContext(
                UUID.randomUUID(),
                hero.getId(),
                hero.getName(),
                1,
                0,
                100,
                hero.getMaxHealth(),
                hero.getMaxHealth(),
                hero.getAttack(),
                hero.getDefense(),
                hero.getMagic(),
                encounters,
                new HashSet<>(),
                learnedMoves,
                new ArrayList<>(equippedMoveIds),
                false
        );

        runs.put(context.runId(), context);
        return toRunConfig(context);
    }

    public RunConfigDTO getRunConfig(UUID runId) {
        return toRunConfig(getRunContext(runId));
    }

    public RunConfigDTO equipMoves(UUID runId, List<Long> moveIds) {
        RunContext context = getRunContext(runId);
        if (moveIds == null || moveIds.isEmpty()) {
            throw new BadRequestException("At least one equipped move is required");
        }
        if (moveIds.size() > MAX_EQUIPPED_MOVES) {
            throw new BadRequestException("You can equip at most " + MAX_EQUIPPED_MOVES + " moves");
        }

        Set<Long> unique = new HashSet<>(moveIds);
        if (unique.size() != moveIds.size()) {
            throw new BadRequestException("Equipped move list contains duplicates");
        }

        for (Long moveId : moveIds) {
            if (!context.learnedMoves().containsKey(moveId)) {
                throw new BadRequestException("Move is not learned yet: " + moveId);
            }
        }

        context.equippedMoveIds().clear();
        context.equippedMoveIds().addAll(moveIds);
        return toRunConfig(context);
    }

    public RunStartEncounterResponseDTO startEncounter(UUID runId, int encounterIndex) {
        RunContext context = getRunContext(runId);
        validateEncounterIndex(context, encounterIndex);
        if (!isEncounterUnlocked(context, encounterIndex)) {
            throw new BadRequestException("Encounter is locked: " + encounterIndex);
        }

        Monster monster = context.encounters().get(encounterIndex);
        BattleSessionCreateRequestDTO createRequest = new BattleSessionCreateRequestDTO(
                context.heroId(),
                monster.getId(),
                context.maxHealth(),
                context.maxHealth(),
                context.attack(),
                context.defense(),
                context.magic(),
                new ArrayList<>(context.equippedMoveIds())
        );

        BattleSessionDTO battleSession = battleSessionService.createSession(createRequest);
        return new RunStartEncounterResponseDTO(toRunConfig(context), battleSession);
    }

    public RunResolveEncounterResponseDTO resolveEncounter(UUID runId, int encounterIndex, boolean heroWon) {
        RunContext context = getRunContext(runId);
        validateEncounterIndex(context, encounterIndex);

        int xpGained = 0;
        int levelsGained = 0;
        MoveDTO learnedMove = null;

        if (heroWon) {
            context.defeatedEncounterIndices().add(encounterIndex);
            Monster encounterMonster = context.encounters().get(encounterIndex);

            xpGained = 50 + (encounterMonster.getDifficultyRank() * 30);
            context.xp(context.xp() + xpGained);

            while (context.xp() >= context.xpToNextLevel()) {
                context.xp(context.xp() - context.xpToNextLevel());
                context.level(context.level() + 1);
                context.xpToNextLevel((int) Math.ceil(context.xpToNextLevel() * 1.35));
                context.maxHealth(context.maxHealth() + 15);
                context.currentHealth(context.maxHealth());
                context.attack(context.attack() + 3);
                context.defense(context.defense() + 3);
                context.magic(context.magic() + 3);
                levelsGained++;
            }

            Optional<Move> learned = learnRandomMonsterMove(context, encounterMonster);
            learnedMove = learned.map(this::toMoveDTO).orElse(null);

            if (context.defeatedEncounterIndices().size() >= context.encounters().size()) {
                context.completed(true);
            }
        }

        return new RunResolveEncounterResponseDTO(
                toRunConfig(context),
                learnedMove,
                xpGained,
                levelsGained
        );
    }

    private Optional<Move> learnRandomMonsterMove(RunContext context, Monster monster) {
        List<Move> candidates = monster.getMoves().stream()
                .filter(move -> !context.learnedMoves().containsKey(move.getId()))
                .toList();

        if (candidates.isEmpty()) {
            return Optional.empty();
        }

        Move selected = candidates.get(ThreadLocalRandom.current().nextInt(candidates.size()));
        context.learnedMoves().put(selected.getId(), selected);
        return Optional.of(selected);
    }

    private void validateEncounterIndex(RunContext context, int encounterIndex) {
        if (encounterIndex < 0 || encounterIndex >= context.encounters().size()) {
            throw new BadRequestException("Encounter index out of range: " + encounterIndex);
        }
    }

    private boolean isEncounterUnlocked(RunContext context, int encounterIndex) {
        if (encounterIndex == 0) {
            return true;
        }
        if (context.defeatedEncounterIndices().contains(encounterIndex)) {
            return true;
        }
        return context.defeatedEncounterIndices().contains(encounterIndex - 1);
    }

    private RunConfigDTO toRunConfig(RunContext context) {
        List<RunEncounterDTO> encounters = new ArrayList<>();
        for (int i = 0; i < context.encounters().size(); i++) {
            Monster monster = context.encounters().get(i);
            encounters.add(new RunEncounterDTO(
                    i,
                    monster.getId(),
                    monster.getName(),
                    monster.getDifficultyRank(),
                    monster.getMaxHealth(),
                    monster.getAttack(),
                    monster.getDefense(),
                    monster.getMagic(),
                    monster.getMoves().stream().map(this::toMoveDTO).toList(),
                    context.defeatedEncounterIndices().contains(i),
                    isEncounterUnlocked(context, i)
            ));
        }

        RunHeroProgressDTO hero = new RunHeroProgressDTO(
                context.heroId(),
                context.heroName(),
                context.level(),
                context.xp(),
                context.xpToNextLevel(),
                context.maxHealth(),
                context.currentHealth(),
                context.attack(),
                context.defense(),
                context.magic()
        );

        return new RunConfigDTO(
                context.runId(),
                hero,
                encounters,
                context.learnedMoves().values().stream().map(this::toMoveDTO).toList(),
                new ArrayList<>(context.equippedMoveIds()),
                context.completed()
        );
    }

    private MoveDTO toMoveDTO(Move move) {
        return new MoveDTO(
                move.getId(),
                move.getName(),
                move.getMoveType(),
                move.getEffectType(),
                move.getPower(),
                move.getBuffAttribute(),
                move.getBuffTarget(),
                move.getBuffAmount(),
                move.getBuffDuration()
        );
    }

    private RunContext getRunContext(UUID runId) {
        return Optional.ofNullable(runs.get(runId))
                .orElseThrow(() -> new ResourceNotFoundException("Run not found: " + runId));
    }

    private static final class RunContext {

        private final UUID runId;
        private final Long heroId;
        private final String heroName;
        private int level;
        private int xp;
        private int xpToNextLevel;
        private int maxHealth;
        private int currentHealth;
        private int attack;
        private int defense;
        private int magic;
        private final List<Monster> encounters;
        private final Set<Integer> defeatedEncounterIndices;
        private final LinkedHashMap<Long, Move> learnedMoves;
        private final List<Long> equippedMoveIds;
        private boolean completed;

        private RunContext(
                UUID runId,
                Long heroId,
                String heroName,
                int level,
                int xp,
                int xpToNextLevel,
                int maxHealth,
                int currentHealth,
                int attack,
                int defense,
                int magic,
                List<Monster> encounters,
                Set<Integer> defeatedEncounterIndices,
                LinkedHashMap<Long, Move> learnedMoves,
                List<Long> equippedMoveIds,
                boolean completed
        ) {
            this.runId = runId;
            this.heroId = heroId;
            this.heroName = heroName;
            this.level = level;
            this.xp = xp;
            this.xpToNextLevel = xpToNextLevel;
            this.maxHealth = maxHealth;
            this.currentHealth = currentHealth;
            this.attack = attack;
            this.defense = defense;
            this.magic = magic;
            this.encounters = encounters;
            this.defeatedEncounterIndices = defeatedEncounterIndices;
            this.learnedMoves = learnedMoves;
            this.equippedMoveIds = equippedMoveIds;
            this.completed = completed;
        }

        public UUID runId() {
            return runId;
        }

        public Long heroId() {
            return heroId;
        }

        public String heroName() {
            return heroName;
        }

        public int level() {
            return level;
        }

        public void level(int level) {
            this.level = level;
        }

        public int xp() {
            return xp;
        }

        public void xp(int xp) {
            this.xp = xp;
        }

        public int xpToNextLevel() {
            return xpToNextLevel;
        }

        public void xpToNextLevel(int xpToNextLevel) {
            this.xpToNextLevel = xpToNextLevel;
        }

        public int maxHealth() {
            return maxHealth;
        }

        public void maxHealth(int maxHealth) {
            this.maxHealth = maxHealth;
        }

        public int currentHealth() {
            return currentHealth;
        }

        public void currentHealth(int currentHealth) {
            this.currentHealth = currentHealth;
        }

        public int attack() {
            return attack;
        }

        public void attack(int attack) {
            this.attack = attack;
        }

        public int defense() {
            return defense;
        }

        public void defense(int defense) {
            this.defense = defense;
        }

        public int magic() {
            return magic;
        }

        public void magic(int magic) {
            this.magic = magic;
        }

        public List<Monster> encounters() {
            return encounters;
        }

        public Set<Integer> defeatedEncounterIndices() {
            return defeatedEncounterIndices;
        }

        public LinkedHashMap<Long, Move> learnedMoves() {
            return learnedMoves;
        }

        public List<Long> equippedMoveIds() {
            return equippedMoveIds;
        }

        public boolean completed() {
            return completed;
        }

        public void completed(boolean completed) {
            this.completed = completed;
        }
    }
}
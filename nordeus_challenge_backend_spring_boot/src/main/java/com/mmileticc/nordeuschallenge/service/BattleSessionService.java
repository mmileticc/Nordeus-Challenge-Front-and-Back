package com.mmileticc.nordeuschallenge.service;

import com.mmileticc.nordeuschallenge.exeption.ResourceNotFoundException;
import com.mmileticc.nordeuschallenge.model.dto.BattleSessionDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleSessionCreateRequestDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleSessionTurnRequestDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleStateDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleTurnResultDTO;
import com.mmileticc.nordeuschallenge.model.dto.HeroDTO;
import com.mmileticc.nordeuschallenge.model.dto.MonsterDTO;
import com.mmileticc.nordeuschallenge.model.dto.MoveDTO;
import com.mmileticc.nordeuschallenge.model.entity.Hero;
import com.mmileticc.nordeuschallenge.model.entity.Monster;
import com.mmileticc.nordeuschallenge.model.entity.Move;
import com.mmileticc.nordeuschallenge.repository.HeroRepository;
import com.mmileticc.nordeuschallenge.repository.MoveRepository;
import com.mmileticc.nordeuschallenge.repository.MonsterRepository;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;

@Service
public class BattleSessionService {

    private final HeroRepository heroRepository;
    private final MonsterRepository monsterRepository;
    private final MoveRepository moveRepository;
    private final BattleService battleService;
    private final ConcurrentMap<UUID, SessionContext> sessionStates = new ConcurrentHashMap<>();
    private final long ttlMs;

    public BattleSessionService(
            HeroRepository heroRepository,
            MonsterRepository monsterRepository,
            MoveRepository moveRepository,
            BattleService battleService,
            @Value("${battle.session.ttl-ms:1800000}") long ttlMs
    ) {
        this.heroRepository = heroRepository;
        this.monsterRepository = monsterRepository;
        this.moveRepository = moveRepository;
        this.battleService = battleService;
        this.ttlMs = ttlMs;
    }

    public BattleSessionDTO createSession(BattleSessionCreateRequestDTO request) {
        Hero hero = heroRepository.findById(request.getHeroId())
                .orElseThrow(() -> new ResourceNotFoundException("Hero not found: " + request.getHeroId()));
        Monster monster = monsterRepository.findById(request.getMonsterId())
                .orElseThrow(() -> new ResourceNotFoundException("Monster not found: " + request.getMonsterId()));

        UUID sessionId = UUID.randomUUID();
        BattleStateDTO battleState = new BattleStateDTO(
                hero.getId(),
                monster.getId(),
                null,
            defaulted(request.getHeroMaxHealth(), hero.getMaxHealth()),
            defaulted(request.getHeroMoveIds(), hero.getMoves().stream().map(Move::getId).toList()),
            defaulted(request.getHeroCurrentHealth(), hero.getCurrentHealth()),
            defaulted(request.getHeroAttack(), hero.getAttack()),
            defaulted(request.getHeroDefense(), hero.getDefense()),
            defaulted(request.getHeroMagic(), hero.getMagic()),
                monster.getCurrentHealth(),
                monster.getAttack(),
                monster.getDefense(),
                monster.getMagic(),
                List.of()
        );

        sessionStates.put(sessionId, new SessionContext(copyBattleState(battleState), System.currentTimeMillis()));

        return new BattleSessionDTO(sessionId, toHeroDTO(hero, battleState.getHeroMoveIds()), toMonsterDTO(monster), battleState);
    }

    public BattleSessionDTO getSession(UUID sessionId) {
        SessionContext context = getSessionContext(sessionId);
        context.touch();
        BattleStateDTO battleState = copyBattleState(context.state());
        Hero hero = heroRepository.findById(battleState.getHeroId())
                .orElseThrow(() -> new ResourceNotFoundException("Hero not found: " + battleState.getHeroId()));
        Monster monster = monsterRepository.findById(battleState.getMonsterId())
                .orElseThrow(() -> new ResourceNotFoundException("Monster not found: " + battleState.getMonsterId()));
        return new BattleSessionDTO(sessionId, toHeroDTO(hero, battleState.getHeroMoveIds()), toMonsterDTO(monster), battleState);
    }

    public BattleTurnResultDTO playTurn(UUID sessionId, BattleSessionTurnRequestDTO request) {
        SessionContext context = getSessionContext(sessionId);
        synchronized (context.lock()) {
            context.touch();
            BattleStateDTO battleState = copyBattleState(context.state());
            battleState.setSelectedMoveId(request.getSelectedMoveId());
            BattleTurnResultDTO result = battleService.executeTurn(battleState);
            context.state(toBattleState(result, battleState));
            context.touch();
            return result;
        }
    }

    public BattleSessionDTO playTurnAndReturnSession(UUID sessionId, BattleSessionTurnRequestDTO request) {
        playTurn(sessionId, request);
        return getSession(sessionId);
    }

    public void deleteSession(UUID sessionId) {
        SessionContext removed = sessionStates.remove(sessionId);
        if (removed == null) {
            throw new ResourceNotFoundException("Battle session not found: " + sessionId);
        }
    }

    @Scheduled(fixedDelayString = "${battle.session.cleanup-ms:60000}")
    public void cleanupExpiredSessions() {
        long now = System.currentTimeMillis();
        sessionStates.entrySet().removeIf(entry -> (now - entry.getValue().lastAccess()) > ttlMs);
    }

    private SessionContext getSessionContext(UUID sessionId) {
        return Optional.ofNullable(sessionStates.get(sessionId))
                .orElseThrow(() -> new ResourceNotFoundException("Battle session not found: " + sessionId));
    }

    private BattleStateDTO toBattleState(BattleTurnResultDTO result, BattleStateDTO previousState) {
        BattleStateDTO battleState = new BattleStateDTO();
        battleState.setHeroId(result.getHeroId());
        battleState.setMonsterId(result.getMonsterId());
        battleState.setHeroMaxHealth(previousState.getHeroMaxHealth());
        battleState.setHeroMoveIds(new ArrayList<>(Optional.ofNullable(previousState.getHeroMoveIds()).orElse(List.of())));
        battleState.setHeroCurrentHealth(result.getHeroCurrentHealth());
        battleState.setHeroAttack(result.getHeroAttack());
        battleState.setHeroDefense(result.getHeroDefense());
        battleState.setHeroMagic(result.getHeroMagic());
        battleState.setMonsterCurrentHealth(result.getMonsterCurrentHealth());
        battleState.setMonsterAttack(result.getMonsterAttack());
        battleState.setMonsterDefense(result.getMonsterDefense());
        battleState.setMonsterMagic(result.getMonsterMagic());
        battleState.setActiveBuffs(new ArrayList<>(Optional.ofNullable(result.getActiveBuffs()).orElse(List.of())));
        return battleState;
    }

    private BattleStateDTO copyBattleState(BattleStateDTO source) {
        BattleStateDTO copy = new BattleStateDTO();
        copy.setHeroId(source.getHeroId());
        copy.setMonsterId(source.getMonsterId());
        copy.setSelectedMoveId(source.getSelectedMoveId());
        copy.setHeroMaxHealth(source.getHeroMaxHealth());
        copy.setHeroMoveIds(new ArrayList<>(Optional.ofNullable(source.getHeroMoveIds()).orElse(List.of())));
        copy.setHeroCurrentHealth(source.getHeroCurrentHealth());
        copy.setHeroAttack(source.getHeroAttack());
        copy.setHeroDefense(source.getHeroDefense());
        copy.setHeroMagic(source.getHeroMagic());
        copy.setMonsterCurrentHealth(source.getMonsterCurrentHealth());
        copy.setMonsterAttack(source.getMonsterAttack());
        copy.setMonsterDefense(source.getMonsterDefense());
        copy.setMonsterMagic(source.getMonsterMagic());
        copy.setActiveBuffs(new ArrayList<>(Optional.ofNullable(source.getActiveBuffs()).orElse(List.of())));
        return copy;
    }

        private HeroDTO toHeroDTO(Hero hero, List<Long> moveIds) {
        List<Long> effectiveMoveIds = moveIds == null || moveIds.isEmpty()
            ? hero.getMoves().stream().map(Move::getId).toList()
            : moveIds;
        List<MoveDTO> moves = moveRepository.findAllById(effectiveMoveIds).stream()
            .map(this::toMoveDTO)
            .toList();

        return new HeroDTO(
                hero.getId(),
                hero.getName(),
                hero.getMaxHealth(),
                hero.getCurrentHealth(),
                hero.getAttack(),
                hero.getDefense(),
                hero.getMagic(),
            moves
        );
    }

    private MonsterDTO toMonsterDTO(Monster monster) {
        return new MonsterDTO(
                monster.getId(),
                monster.getName(),
                monster.getDifficultyRank(),
                monster.getMaxHealth(),
                monster.getAttack(),
                monster.getDefense(),
                monster.getMagic()
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

    private int defaulted(Integer value, int fallback) {
        return value == null ? fallback : value;
    }

    private List<Long> defaulted(List<Long> value, List<Long> fallback) {
        return (value == null || value.isEmpty()) ? fallback : value;
    }

    private static class SessionContext {

        private final Object lock = new Object();
        private volatile BattleStateDTO state;
        private volatile long lastAccess;

        private SessionContext(BattleStateDTO state, long lastAccess) {
            this.state = state;
            this.lastAccess = lastAccess;
        }

        public Object lock() {
            return lock;
        }

        public BattleStateDTO state() {
            return state;
        }

        public void state(BattleStateDTO state) {
            this.state = state;
        }

        public long lastAccess() {
            return lastAccess;
        }

        public void touch() {
            this.lastAccess = System.currentTimeMillis();
        }
    }
}

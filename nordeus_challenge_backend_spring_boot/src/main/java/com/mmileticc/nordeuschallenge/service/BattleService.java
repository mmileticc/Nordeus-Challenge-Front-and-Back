package com.mmileticc.nordeuschallenge.service;

import com.mmileticc.nordeuschallenge.exeption.BadRequestException;
import com.mmileticc.nordeuschallenge.exeption.ResourceNotFoundException;
import com.mmileticc.nordeuschallenge.model.battle.BattleCharacter;
import com.mmileticc.nordeuschallenge.model.dto.ActiveBuffDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleStateDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleTurnResultDTO;
import com.mmileticc.nordeuschallenge.model.entity.Hero;
import com.mmileticc.nordeuschallenge.model.entity.Monster;
import com.mmileticc.nordeuschallenge.model.entity.Move;
import com.mmileticc.nordeuschallenge.model.entity.enums.BuffAttribute;
import com.mmileticc.nordeuschallenge.model.entity.enums.BuffTarget;
import com.mmileticc.nordeuschallenge.model.entity.enums.CharacterSide;
import com.mmileticc.nordeuschallenge.model.entity.enums.EffectType;
import com.mmileticc.nordeuschallenge.model.entity.enums.MoveType;
import com.mmileticc.nordeuschallenge.repository.HeroRepository;
import com.mmileticc.nordeuschallenge.repository.MoveRepository;
import com.mmileticc.nordeuschallenge.repository.MonsterRepository;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.stream.Collectors;

@Service
public class BattleService {

    private final HeroRepository heroRepository;
    private final MonsterRepository monsterRepository;
    private final MoveRepository moveRepository;
    private final MoveEffectResolver moveEffectResolver;
    private final MonsterAiService monsterAiService;

    public BattleService(
            HeroRepository heroRepository,
            MonsterRepository monsterRepository,
            MoveRepository moveRepository,
            MoveEffectResolver moveEffectResolver,
            MonsterAiService monsterAiService
    ) {
        this.heroRepository = heroRepository;
        this.monsterRepository = monsterRepository;
        this.moveRepository = moveRepository;
        this.moveEffectResolver = moveEffectResolver;
        this.monsterAiService = monsterAiService;
    }

    public BattleTurnResultDTO executeTurn(BattleStateDTO battleState) {
        Hero hero = heroRepository.findById(battleState.getHeroId())
                .orElseThrow(() -> new ResourceNotFoundException("Hero not found: " + battleState.getHeroId()));
        Monster monster = monsterRepository.findById(battleState.getMonsterId())
                .orElseThrow(() -> new ResourceNotFoundException("Monster not found: " + battleState.getMonsterId()));

        BattleCharacter heroState = toHeroState(hero, battleState);
        BattleCharacter monsterState = toMonsterState(monster, battleState);

        List<ActiveBuffDTO> activeBuffs = new ArrayList<>(
                Optional.ofNullable(battleState.getActiveBuffs()).orElseGet(ArrayList::new)
        );
        List<String> logs = new ArrayList<>();

        Map<Long, Move> allowedHeroMoves = loadHeroAllowedMoves(hero, battleState);

        Move heroMove = allowedHeroMoves.values().stream()
                .filter(move -> move.getId().equals(battleState.getSelectedMoveId()))
                .findFirst()
                .orElseThrow(() -> new BadRequestException("Selected move is not available for hero"));

        applyMove(heroMove, heroState, monsterState, CharacterSide.HERO, activeBuffs, logs);

        String monsterMoveName = null;
        if (monsterState.getCurrentHealth() > 0) {
            Optional<Move> monsterMove = monsterAiService.chooseMove(monster, monsterState.getCurrentHealth());
            if (monsterMove.isPresent()) {
                monsterMoveName = monsterMove.get().getName();
                applyMove(monsterMove.get(), monsterState, heroState, CharacterSide.MONSTER, activeBuffs, logs);
            }
        }

        tickAndRevertExpiredBuffs(activeBuffs, heroState, monsterState);

        BattleTurnResultDTO result = new BattleTurnResultDTO();
        result.setHeroId(hero.getId());
        result.setMonsterId(monster.getId());
        result.setHeroCurrentHealth(heroState.getCurrentHealth());
        result.setHeroAttack(heroState.getAttack());
        result.setHeroDefense(heroState.getDefense());
        result.setHeroMagic(heroState.getMagic());
        result.setMonsterCurrentHealth(monsterState.getCurrentHealth());
        result.setMonsterAttack(monsterState.getAttack());
        result.setMonsterDefense(monsterState.getDefense());
        result.setMonsterMagic(monsterState.getMagic());
        result.setHeroMoveName(heroMove.getName());
        result.setMonsterMoveName(monsterMoveName);
        result.setLogs(logs);
        result.setActiveBuffs(activeBuffs);

        if (heroState.getCurrentHealth() <= 0 || monsterState.getCurrentHealth() <= 0) {
            result.setBattleFinished(true);
            result.setWinner(heroState.getCurrentHealth() > 0 ? "HERO" : "MONSTER");
        } else {
            result.setBattleFinished(false);
            result.setWinner("NONE");
        }

        return result;
    }

    private void applyMove(
            Move move,
            BattleCharacter caster,
            BattleCharacter target,
            CharacterSide casterSide,
            List<ActiveBuffDTO> activeBuffs,
            List<String> logs
    ) {
        EffectType effectType = move.getEffectType();
        BattleCharacter effectTarget = resolveEffectTarget(move, caster, target);

        int baseValue = calculateBaseValue(move, caster, target);
        moveEffectResolver.resolve(effectType).execute(caster, effectTarget, baseValue);

        if (effectType == EffectType.BUFF) {
            CharacterSide buffTargetSide = effectTarget == caster ? casterSide : opposite(casterSide);
            ActiveBuffDTO buff = new ActiveBuffDTO(
                    buffTargetSide,
                    move.getBuffAttribute(),
                    move.getBuffAmount(),
                    Math.max(1, move.getBuffDuration())
            );
            activeBuffs.add(buff);
            logs.add(caster.getName() + " used " + move.getName() + " and "
                    + (move.getBuffAmount() >= 0 ? "modified" : "reduced") + " "
                    + (buffTargetSide == casterSide ? "own" : "enemy") + " "
                    + move.getBuffAttribute() + " by " + move.getBuffAmount() + " for "
                    + buff.getRemainingTurns() + " turns.");
        } else {
            logs.add(caster.getName() + " used " + move.getName() + " for " + Math.max(1, baseValue) + " effect value.");
        }
    }

    private CharacterSide opposite(CharacterSide side) {
        return side == CharacterSide.HERO ? CharacterSide.MONSTER : CharacterSide.HERO;
    }

    private BattleCharacter resolveEffectTarget(Move move, BattleCharacter caster, BattleCharacter target) {
        if (move.getEffectType() == EffectType.DAMAGE) {
            return target;
        }
        if (move.getEffectType() == EffectType.HEAL) {
            return caster;
        }
        if (move.getBuffTarget() == BuffTarget.OPPONENT) {
            return target;
        }
        return caster;
    }

    private int calculateBaseValue(Move move, BattleCharacter caster, BattleCharacter target) {
        return switch (move.getEffectType()) {
            case DAMAGE -> {
                int offense = move.getMoveType() == MoveType.MAGIC ? caster.getMagic() : caster.getAttack();
                int defense = move.getMoveType() == MoveType.MAGIC ? 0 : target.getDefense();
                yield Math.max(1, move.getPower() + offense - defense);
            }
            case HEAL -> Math.max(1, move.getPower() + (caster.getMagic() / 2));
            case BUFF -> encodeBuffBaseValue(move.getBuffAttribute(), move.getBuffAmount());
        };
    }

    private int encodeBuffBaseValue(BuffAttribute attribute, int amount) {
        return (attribute.ordinal() * 1000) + amount;
    }

    private void tickAndRevertExpiredBuffs(List<ActiveBuffDTO> activeBuffs, BattleCharacter heroState, BattleCharacter monsterState) {
        List<ActiveBuffDTO> expired = new ArrayList<>();

        for (ActiveBuffDTO buff : activeBuffs) {
            buff.setRemainingTurns(buff.getRemainingTurns() - 1);
            if (buff.getRemainingTurns() <= 0) {
                BattleCharacter target = buff.getTargetSide() == CharacterSide.HERO ? heroState : monsterState;
                revertBuff(target, buff);
                expired.add(buff);
            }
        }

        activeBuffs.removeAll(expired);
    }

    private void revertBuff(BattleCharacter target, ActiveBuffDTO buff) {
        switch (buff.getAttribute()) {
            case ATTACK -> target.setAttack(Math.max(0, target.getAttack() - buff.getAmount()));
            case DEFENSE -> target.setDefense(Math.max(0, target.getDefense() - buff.getAmount()));
            case MAGIC -> target.setMagic(Math.max(0, target.getMagic() - buff.getAmount()));
            default -> throw new IllegalStateException("Unsupported buff attribute: " + buff.getAttribute());
        }
    }

    private BattleCharacter toHeroState(Hero hero, BattleStateDTO state) {
        return new BattleCharacter(
                hero.getId(),
                hero.getName(),
            defaulted(state.getHeroMaxHealth(), hero.getMaxHealth()),
                defaulted(state.getHeroCurrentHealth(), hero.getCurrentHealth()),
                defaulted(state.getHeroAttack(), hero.getAttack()),
                defaulted(state.getHeroDefense(), hero.getDefense()),
                defaulted(state.getHeroMagic(), hero.getMagic())
        );
    }

    private BattleCharacter toMonsterState(Monster monster, BattleStateDTO state) {
        return new BattleCharacter(
                monster.getId(),
                monster.getName(),
                monster.getMaxHealth(),
                defaulted(state.getMonsterCurrentHealth(), monster.getCurrentHealth()),
                defaulted(state.getMonsterAttack(), monster.getAttack()),
                defaulted(state.getMonsterDefense(), monster.getDefense()),
                defaulted(state.getMonsterMagic(), monster.getMagic())
        );
    }

    private int defaulted(Integer candidate, int fallback) {
        return candidate == null ? fallback : candidate;
    }

    private Map<Long, Move> loadHeroAllowedMoves(Hero hero, BattleStateDTO battleState) {
        List<Long> overrideMoveIds = Optional.ofNullable(battleState.getHeroMoveIds()).orElse(Collections.emptyList());
        if (overrideMoveIds.isEmpty()) {
            return hero.getMoves().stream().collect(Collectors.toMap(Move::getId, move -> move));
        }

        Map<Long, Move> moveMap = new HashMap<>();
        for (Move move : moveRepository.findAllById(overrideMoveIds)) {
            moveMap.put(move.getId(), move);
        }
        return moveMap;
    }

    public Optional<Move> chooseMonsterMove(Long monsterId, int currentHealth) {
        Monster monster = monsterRepository.findById(monsterId)
                .orElseThrow(() -> new ResourceNotFoundException("Monster not found: " + monsterId));
        return monsterAiService.chooseMove(monster, currentHealth);
    }
}

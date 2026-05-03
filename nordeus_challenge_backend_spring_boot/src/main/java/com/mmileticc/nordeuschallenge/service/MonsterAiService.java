package com.mmileticc.nordeuschallenge.service;

import com.mmileticc.nordeuschallenge.model.entity.Monster;
import com.mmileticc.nordeuschallenge.model.entity.Move;
import com.mmileticc.nordeuschallenge.model.entity.enums.EffectType;
import org.springframework.stereotype.Service;

import java.util.Comparator;
import java.util.Optional;

@Service
public class MonsterAiService {

    public Optional<Move> chooseMove(Monster monster, int currentHealth) {
        if (monster.getMoves() == null || monster.getMoves().isEmpty()) {
            return Optional.empty();
        }

        double hpRatio = (double) currentHealth / Math.max(1, monster.getMaxHealth());
        if (hpRatio < 0.30) {
            Optional<Move> healMove = monster.getMoves().stream()
                    .filter(move -> move.getEffectType() == EffectType.HEAL)
                    .max(Comparator.comparingInt(Move::getPower));
            if (healMove.isPresent()) {
                return healMove;
            }
        }

        return monster.getMoves().stream()
                .max(Comparator.comparingInt(Move::getPower));
    }
}

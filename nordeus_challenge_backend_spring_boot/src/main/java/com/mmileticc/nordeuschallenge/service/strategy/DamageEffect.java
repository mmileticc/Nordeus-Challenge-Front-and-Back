package com.mmileticc.nordeuschallenge.service.strategy;

import com.mmileticc.nordeuschallenge.model.entity.BaseCharacter;
import com.mmileticc.nordeuschallenge.model.entity.enums.EffectType;
import org.springframework.stereotype.Component;

@Component
public class DamageEffect implements MoveEffectStrategy {

    @Override
    public void execute(BaseCharacter caster, BaseCharacter target, int baseValue) {
        int nextHealth = Math.max(0, target.getCurrentHealth() - Math.max(1, baseValue));
        target.setCurrentHealth(nextHealth);
    }

    @Override
    public EffectType getSupportedEffectType() {
        return EffectType.DAMAGE;
    }
}

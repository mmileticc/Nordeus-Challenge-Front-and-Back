package com.mmileticc.nordeuschallenge.service.strategy;

import com.mmileticc.nordeuschallenge.model.entity.BaseCharacter;
import com.mmileticc.nordeuschallenge.model.entity.enums.EffectType;

public interface MoveEffectStrategy {

    void execute(BaseCharacter caster, BaseCharacter target, int baseValue);

    EffectType getSupportedEffectType();
}

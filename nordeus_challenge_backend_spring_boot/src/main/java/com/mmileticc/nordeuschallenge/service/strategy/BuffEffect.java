package com.mmileticc.nordeuschallenge.service.strategy;

import com.mmileticc.nordeuschallenge.model.entity.BaseCharacter;
import com.mmileticc.nordeuschallenge.model.entity.enums.BuffAttribute;
import com.mmileticc.nordeuschallenge.model.entity.enums.EffectType;
import org.springframework.stereotype.Component;

@Component
public class BuffEffect implements MoveEffectStrategy {

    @Override
    public void execute(BaseCharacter caster, BaseCharacter target, int baseValue) {
        int attributeCode = baseValue / 1000;
        int amount = baseValue - (attributeCode * 1000);
        BuffAttribute attribute = BuffAttribute.values()[attributeCode];

        switch (attribute) {
            case ATTACK -> target.setAttack(target.getAttack() + amount);
            case DEFENSE -> target.setDefense(target.getDefense() + amount);
            case MAGIC -> target.setMagic(target.getMagic() + amount);
            default -> throw new IllegalStateException("Unsupported buff attribute: " + attribute);
        }
    }

    @Override
    public EffectType getSupportedEffectType() {
        return EffectType.BUFF;
    }
}

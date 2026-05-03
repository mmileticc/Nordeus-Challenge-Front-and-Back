package com.mmileticc.nordeuschallenge.model.battle;

import com.mmileticc.nordeuschallenge.model.entity.BaseCharacter;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.EqualsAndHashCode;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@EqualsAndHashCode(callSuper = true)
public class BattleCharacter extends BaseCharacter {

    private Long id;
    private String name;

    public BattleCharacter(
            Long id,
            String name,
            int maxHealth,
            int currentHealth,
            int attack,
            int defense,
            int magic
    ) {
        super(maxHealth, currentHealth, attack, defense, magic);
        this.id = id;
        this.name = name;
    }
}

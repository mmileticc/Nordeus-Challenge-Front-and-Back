package com.mmileticc.nordeuschallenge.model.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.ArrayList;
import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class BattleStateDTO {

    private Long heroId;
    private Long monsterId;
    private Long selectedMoveId;
    private Integer heroMaxHealth;
    private List<Long> heroMoveIds = new ArrayList<>();

    private Integer heroCurrentHealth;
    private Integer heroAttack;
    private Integer heroDefense;
    private Integer heroMagic;

    private Integer monsterCurrentHealth;
    private Integer monsterAttack;
    private Integer monsterDefense;
    private Integer monsterMagic;

    private List<ActiveBuffDTO> activeBuffs = new ArrayList<>();
}

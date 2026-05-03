package com.mmileticc.nordeuschallenge.model.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.ArrayList;
import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class BattleTurnResultDTO {

    private Long heroId;
    private Long monsterId;

    private int heroCurrentHealth;
    private int heroAttack;
    private int heroDefense;
    private int heroMagic;

    private int monsterCurrentHealth;
    private int monsterAttack;
    private int monsterDefense;
    private int monsterMagic;

    private String heroMoveName;
    private String monsterMoveName;

    private boolean battleFinished;
    private String winner;

    private List<String> logs = new ArrayList<>();
    private List<ActiveBuffDTO> activeBuffs = new ArrayList<>();
}

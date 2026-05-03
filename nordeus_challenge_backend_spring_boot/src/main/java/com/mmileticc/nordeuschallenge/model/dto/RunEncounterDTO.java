package com.mmileticc.nordeuschallenge.model.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class RunEncounterDTO {

    private int encounterIndex;
    private Long monsterId;
    private String monsterName;
    private int difficultyRank;
    private int maxHealth;
    private int attack;
    private int defense;
    private int magic;
    private List<MoveDTO> moves;
    private boolean defeated;
    private boolean unlocked;
}
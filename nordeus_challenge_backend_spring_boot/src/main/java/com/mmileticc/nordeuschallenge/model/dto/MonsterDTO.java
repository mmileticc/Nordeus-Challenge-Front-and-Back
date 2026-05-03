package com.mmileticc.nordeuschallenge.model.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class MonsterDTO {

    private Long id;
    private String name;
    private int difficultyRank;
    private int maxHealth;
    private int attack;
    private int defense;
    private int magic;
}

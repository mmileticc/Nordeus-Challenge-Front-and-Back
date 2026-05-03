package com.mmileticc.nordeuschallenge.model.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class RunHeroProgressDTO {

    private Long heroId;
    private String heroName;
    private int level;
    private int xp;
    private int xpToNextLevel;
    private int maxHealth;
    private int currentHealth;
    private int attack;
    private int defense;
    private int magic;
}
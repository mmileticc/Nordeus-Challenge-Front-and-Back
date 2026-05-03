package com.mmileticc.nordeuschallenge.model.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class HeroDTO {

    private Long id;
    private String name;
    private int maxHealth;
    private int currentHealth;
    private int attack;
    private int defense;
    private int magic;
    private List<MoveDTO> moves;
}

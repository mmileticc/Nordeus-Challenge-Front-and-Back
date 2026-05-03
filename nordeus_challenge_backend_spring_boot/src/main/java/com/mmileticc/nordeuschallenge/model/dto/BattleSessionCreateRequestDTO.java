package com.mmileticc.nordeuschallenge.model.dto;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Positive;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.ArrayList;
import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class BattleSessionCreateRequestDTO {

    @NotNull(message = "heroId is required")
    @Positive(message = "heroId must be positive")
    private Long heroId;

    @NotNull(message = "monsterId is required")
    @Positive(message = "monsterId must be positive")
    private Long monsterId;

    private Integer heroMaxHealth;
    private Integer heroCurrentHealth;
    private Integer heroAttack;
    private Integer heroDefense;
    private Integer heroMagic;
    private List<Long> heroMoveIds = new ArrayList<>();
}

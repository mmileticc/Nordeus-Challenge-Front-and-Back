package com.mmileticc.nordeuschallenge.model.dto;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Positive;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class BattleSessionTurnRequestDTO {

    @NotNull(message = "selectedMoveId is required")
    @Positive(message = "selectedMoveId must be positive")
    private Long selectedMoveId;
}

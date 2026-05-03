package com.mmileticc.nordeuschallenge.model.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;
import java.util.UUID;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class RunConfigDTO {

    private UUID runId;
    private RunHeroProgressDTO hero;
    private List<RunEncounterDTO> encounters;
    private List<MoveDTO> learnedMoves;
    private List<Long> equippedMoveIds;
    private boolean completed;
}
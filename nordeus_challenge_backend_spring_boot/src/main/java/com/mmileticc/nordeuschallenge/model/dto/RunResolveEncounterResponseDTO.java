package com.mmileticc.nordeuschallenge.model.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class RunResolveEncounterResponseDTO {

    private RunConfigDTO run;
    private MoveDTO learnedMove;
    private int xpGained;
    private int levelsGained;
}
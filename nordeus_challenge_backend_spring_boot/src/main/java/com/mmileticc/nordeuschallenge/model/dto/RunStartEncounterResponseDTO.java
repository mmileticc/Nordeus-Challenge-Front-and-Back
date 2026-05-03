package com.mmileticc.nordeuschallenge.model.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class RunStartEncounterResponseDTO {

    private RunConfigDTO run;
    private BattleSessionDTO battleSession;
}
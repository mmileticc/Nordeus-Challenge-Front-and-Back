package com.mmileticc.nordeuschallenge.model.dto;

import jakarta.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class RunResolveEncounterRequestDTO {

    @NotNull(message = "heroWon is required")
    private Boolean heroWon;
}
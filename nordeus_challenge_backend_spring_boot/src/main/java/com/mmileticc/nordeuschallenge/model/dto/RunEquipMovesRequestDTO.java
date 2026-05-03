package com.mmileticc.nordeuschallenge.model.dto;

import jakarta.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.ArrayList;
import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class RunEquipMovesRequestDTO {

    @NotNull(message = "moveIds is required")
    private List<Long> moveIds = new ArrayList<>();
}
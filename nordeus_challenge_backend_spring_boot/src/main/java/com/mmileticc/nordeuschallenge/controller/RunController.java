package com.mmileticc.nordeuschallenge.controller;

import com.mmileticc.nordeuschallenge.model.dto.RunConfigDTO;
import com.mmileticc.nordeuschallenge.model.dto.RunEquipMovesRequestDTO;
import com.mmileticc.nordeuschallenge.model.dto.RunResolveEncounterRequestDTO;
import com.mmileticc.nordeuschallenge.model.dto.RunResolveEncounterResponseDTO;
import com.mmileticc.nordeuschallenge.model.dto.RunStartEncounterResponseDTO;
import com.mmileticc.nordeuschallenge.model.dto.RunStartRequestDTO;
import com.mmileticc.nordeuschallenge.service.RunService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
@RequestMapping("/api/runs")
@Tag(name = "Runs", description = "Run progression, encounters and move management")
public class RunController {

    private final RunService runService;

    public RunController(RunService runService) {
        this.runService = runService;
    }

    @PostMapping("/start")
    @Operation(summary = "Start a new gauntlet run")
    public ResponseEntity<RunConfigDTO> startRun(@RequestBody(required = false) RunStartRequestDTO request) {
        Long heroId = request == null ? null : request.getHeroId();
        return ResponseEntity.ok(runService.startRun(heroId));
    }

    @GetMapping("/{runId}/config")
    @Operation(summary = "Get run configuration and current progression")
    public ResponseEntity<RunConfigDTO> getRunConfig(@PathVariable UUID runId) {
        return ResponseEntity.ok(runService.getRunConfig(runId));
    }

    @PutMapping("/{runId}/moves/equipped")
    @Operation(summary = "Equip moves from learned move list")
    public ResponseEntity<RunConfigDTO> equipMoves(@PathVariable UUID runId, @Valid @RequestBody RunEquipMovesRequestDTO request) {
        return ResponseEntity.ok(runService.equipMoves(runId, request.getMoveIds()));
    }

    @PostMapping("/{runId}/encounters/{encounterIndex}/start")
    @Operation(summary = "Start an encounter battle session")
    public ResponseEntity<RunStartEncounterResponseDTO> startEncounter(
            @PathVariable UUID runId,
            @PathVariable int encounterIndex
    ) {
        return ResponseEntity.ok(runService.startEncounter(runId, encounterIndex));
    }

    @PostMapping("/{runId}/encounters/{encounterIndex}/resolve")
    @Operation(summary = "Resolve encounter outcome and apply progression")
    public ResponseEntity<RunResolveEncounterResponseDTO> resolveEncounter(
            @PathVariable UUID runId,
            @PathVariable int encounterIndex,
            @Valid @RequestBody RunResolveEncounterRequestDTO request
    ) {
        return ResponseEntity.ok(runService.resolveEncounter(runId, encounterIndex, request.getHeroWon()));
    }
}
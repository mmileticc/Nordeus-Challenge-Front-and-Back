package com.mmileticc.nordeuschallenge.controller;

import com.mmileticc.nordeuschallenge.model.dto.BattleSessionDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleSessionCreateRequestDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleSessionTurnRequestDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleTurnResultDTO;
import com.mmileticc.nordeuschallenge.service.BattleSessionService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
@RequestMapping("/api/battle-sessions")
@Tag(name = "Battle Sessions", description = "Session-based RPG battle flow")
public class BattleSessionController {

    private final BattleSessionService battleSessionService;

    public BattleSessionController(BattleSessionService battleSessionService) {
        this.battleSessionService = battleSessionService;
    }

    @PostMapping
    @Operation(summary = "Create a new battle session")
    public ResponseEntity<BattleSessionDTO> createSession(@Valid @RequestBody BattleSessionCreateRequestDTO request) {
        return ResponseEntity.ok(battleSessionService.createSession(request));
    }

    @GetMapping("/{sessionId}")
    @Operation(summary = "Get battle session state")
    public ResponseEntity<BattleSessionDTO> getSession(@PathVariable UUID sessionId) {
        return ResponseEntity.ok(battleSessionService.getSession(sessionId));
    }

    @PostMapping("/{sessionId}/turn")
    @Operation(summary = "Execute a turn in an existing battle session")
    public ResponseEntity<BattleTurnResultDTO> playTurn(
            @PathVariable UUID sessionId,
            @Valid @RequestBody BattleSessionTurnRequestDTO request
    ) {
        return ResponseEntity.ok(battleSessionService.playTurn(sessionId, request));
    }

    @DeleteMapping("/{sessionId}")
    @Operation(summary = "Delete battle session")
    public ResponseEntity<Void> deleteSession(@PathVariable UUID sessionId) {
        battleSessionService.deleteSession(sessionId);
        return ResponseEntity.noContent().build();
    }
}

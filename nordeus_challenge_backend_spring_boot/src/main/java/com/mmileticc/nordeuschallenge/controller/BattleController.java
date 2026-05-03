package com.mmileticc.nordeuschallenge.controller;

import com.mmileticc.nordeuschallenge.model.dto.BattleStateDTO;
import com.mmileticc.nordeuschallenge.model.dto.BattleTurnResultDTO;
import com.mmileticc.nordeuschallenge.model.dto.MoveDTO;
import com.mmileticc.nordeuschallenge.model.entity.Move;
import com.mmileticc.nordeuschallenge.service.BattleService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Optional;

@RestController
@RequestMapping("/api/battle")
@Tag(name = "Battle", description = "Stateless battle calculation endpoint")
public class BattleController {

    private final BattleService battleService;

    public BattleController(BattleService battleService) {
        this.battleService = battleService;
    }

    @PostMapping("/turn")
    @Operation(summary = "Execute one battle turn using the supplied battle state")
    public ResponseEntity<BattleTurnResultDTO> executeTurn(@Valid @RequestBody BattleStateDTO battleStateDTO) {
        return ResponseEntity.ok(battleService.executeTurn(battleStateDTO));
    }

    @GetMapping("/monster-next-move")
    @Operation(summary = "Get monster next move suggestion based on monster and current health")
        public ResponseEntity<MoveDTO> getMonsterNextMove(
            @RequestParam Long monsterId,
            @RequestParam int currentHealth
        ) {
        Optional<Move> move = battleService.chooseMonsterMove(monsterId, currentHealth);
        return move.map(value -> ResponseEntity.ok(toMoveDTO(value)))
                .orElseGet(() -> ResponseEntity.noContent().build());
    }

    private MoveDTO toMoveDTO(Move move) {
        return new MoveDTO(
                move.getId(),
                move.getName(),
                move.getMoveType(),
                move.getEffectType(),
                move.getPower(),
                move.getBuffAttribute(),
                move.getBuffTarget(),
                move.getBuffAmount(),
                move.getBuffDuration()
        );
    }
}

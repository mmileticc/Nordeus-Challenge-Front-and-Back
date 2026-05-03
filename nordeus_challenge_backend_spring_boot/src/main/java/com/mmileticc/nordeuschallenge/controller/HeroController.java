package com.mmileticc.nordeuschallenge.controller;

import com.mmileticc.nordeuschallenge.model.dto.HeroDTO;
import com.mmileticc.nordeuschallenge.model.dto.MoveDTO;
import com.mmileticc.nordeuschallenge.repository.HeroRepository;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/heroes")
@Tag(name = "Heroes", description = "Hero catalogue")
public class HeroController {

    private final HeroRepository heroRepository;

    public HeroController(HeroRepository heroRepository) {
        this.heroRepository = heroRepository;
    }

    @GetMapping
    @Operation(summary = "List all heroes")
    public ResponseEntity<List<HeroDTO>> getAllHeroes() {
        List<HeroDTO> heroes = heroRepository.findAll().stream()
                .map(hero -> new HeroDTO(
                        hero.getId(),
                        hero.getName(),
                        hero.getMaxHealth(),
                        hero.getCurrentHealth(),
                        hero.getAttack(),
                        hero.getDefense(),
                        hero.getMagic(),
                        hero.getMoves().stream()
                                .map(move -> new MoveDTO(
                                        move.getId(),
                                        move.getName(),
                                        move.getMoveType(),
                                        move.getEffectType(),
                                        move.getPower(),
                                        move.getBuffAttribute(),
                                        move.getBuffTarget(),
                                        move.getBuffAmount(),
                                        move.getBuffDuration()
                                ))
                                .toList()
                ))
                .toList();

        return ResponseEntity.ok(heroes);
    }
}

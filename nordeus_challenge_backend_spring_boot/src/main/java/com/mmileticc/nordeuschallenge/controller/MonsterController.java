package com.mmileticc.nordeuschallenge.controller;

import com.mmileticc.nordeuschallenge.model.dto.MonsterDTO;
import com.mmileticc.nordeuschallenge.repository.MonsterRepository;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/api/monsters")
@Tag(name = "Monsters", description = "Monster catalogue")
public class MonsterController {

    private final MonsterRepository monsterRepository;

    public MonsterController(MonsterRepository monsterRepository) {
        this.monsterRepository = monsterRepository;
    }

    @GetMapping
    @Operation(summary = "List all monsters")
    public ResponseEntity<List<MonsterDTO>> getAllMonsters() {
        List<MonsterDTO> monsters = monsterRepository.findAll().stream()
                .map(monster -> new MonsterDTO(
                        monster.getId(),
                        monster.getName(),
                        monster.getDifficultyRank(),
                        monster.getMaxHealth(),
                        monster.getAttack(),
                        monster.getDefense(),
                        monster.getMagic()
                ))
                .toList();

        return ResponseEntity.ok(monsters);
    }
}

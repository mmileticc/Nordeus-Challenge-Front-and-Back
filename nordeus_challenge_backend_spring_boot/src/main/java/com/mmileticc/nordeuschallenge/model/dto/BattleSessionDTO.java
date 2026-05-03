package com.mmileticc.nordeuschallenge.model.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.UUID;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class BattleSessionDTO {

    private UUID sessionId;
    private HeroDTO hero;
    private MonsterDTO monster;
    private BattleStateDTO battleState;
}

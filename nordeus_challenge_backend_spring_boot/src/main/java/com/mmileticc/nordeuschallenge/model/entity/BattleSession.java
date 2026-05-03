package com.mmileticc.nordeuschallenge.model.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.UUID;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Entity
@Table(name = "battle_sessions")
public class BattleSession {

    @Id
    @GeneratedValue
    private UUID id;

    @Column(nullable = false)
    private Long heroId;

    @Column(nullable = false)
    private Long monsterId;

    @Column(nullable = false)
    private int heroCurrentHealth;

    @Column(nullable = false)
    private int heroAttack;

    @Column(nullable = false)
    private int heroDefense;

    @Column(nullable = false)
    private int heroMagic;

    @Column(nullable = false)
    private int monsterCurrentHealth;

    @Column(nullable = false)
    private int monsterAttack;

    @Column(nullable = false)
    private int monsterDefense;

    @Column(nullable = false)
    private int monsterMagic;
}

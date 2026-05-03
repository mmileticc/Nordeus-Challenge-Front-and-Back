package com.mmileticc.nordeuschallenge.model.entity;

import com.mmileticc.nordeuschallenge.model.entity.enums.BuffAttribute;
import com.mmileticc.nordeuschallenge.model.entity.enums.BuffTarget;
import com.mmileticc.nordeuschallenge.model.entity.enums.EffectType;
import com.mmileticc.nordeuschallenge.model.entity.enums.MoveType;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Entity
@Table(name = "moves")
public class Move {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String name;

    @Enumerated(EnumType.STRING)
    private MoveType moveType;

    @Enumerated(EnumType.STRING)
    private EffectType effectType;

    private int power;

    @Enumerated(EnumType.STRING)
    private BuffAttribute buffAttribute;

    @Enumerated(EnumType.STRING)
    private BuffTarget buffTarget;

    private int buffAmount;

    private int buffDuration;
}

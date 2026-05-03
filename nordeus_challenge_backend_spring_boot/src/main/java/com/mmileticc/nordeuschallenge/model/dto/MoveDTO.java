package com.mmileticc.nordeuschallenge.model.dto;

import com.mmileticc.nordeuschallenge.model.entity.enums.BuffAttribute;
import com.mmileticc.nordeuschallenge.model.entity.enums.BuffTarget;
import com.mmileticc.nordeuschallenge.model.entity.enums.EffectType;
import com.mmileticc.nordeuschallenge.model.entity.enums.MoveType;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class MoveDTO {

    private Long id;
    private String name;
    private MoveType moveType;
    private EffectType effectType;
    private int power;
    private BuffAttribute buffAttribute;
    private BuffTarget buffTarget;
    private int buffAmount;
    private int buffDuration;
}

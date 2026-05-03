package com.mmileticc.nordeuschallenge.model.dto;

import com.mmileticc.nordeuschallenge.model.entity.enums.BuffAttribute;
import com.mmileticc.nordeuschallenge.model.entity.enums.CharacterSide;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ActiveBuffDTO {

    private CharacterSide targetSide;
    private BuffAttribute attribute;
    private int amount;
    private int remainingTurns;
}

package com.mmileticc.nordeuschallenge.service;

import com.mmileticc.nordeuschallenge.model.entity.enums.EffectType;
import com.mmileticc.nordeuschallenge.service.strategy.MoveEffectStrategy;
import org.springframework.stereotype.Component;

import java.util.Arrays;
import java.util.Map;
import java.util.function.Function;
import java.util.stream.Collectors;

@Component
public class MoveEffectResolver {

    private final Map<EffectType, MoveEffectStrategy> strategies;

    public MoveEffectResolver(MoveEffectStrategy[] strategies) {
        this.strategies = Arrays.stream(strategies)
                .collect(Collectors.toMap(MoveEffectStrategy::getSupportedEffectType, Function.identity()));
    }

    public MoveEffectStrategy resolve(EffectType effectType) {
        return strategies.get(effectType);
    }
}

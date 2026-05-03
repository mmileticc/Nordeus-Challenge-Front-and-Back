package com.mmileticc.nordeuschallenge.repository;

import com.mmileticc.nordeuschallenge.model.entity.BattleSession;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.UUID;

public interface BattleSessionRepository extends JpaRepository<BattleSession, UUID> {
}

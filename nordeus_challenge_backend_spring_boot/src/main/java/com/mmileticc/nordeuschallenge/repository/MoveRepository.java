package com.mmileticc.nordeuschallenge.repository;

import com.mmileticc.nordeuschallenge.model.entity.Move;
import org.springframework.data.jpa.repository.JpaRepository;

public interface MoveRepository extends JpaRepository<Move, Long> {
}

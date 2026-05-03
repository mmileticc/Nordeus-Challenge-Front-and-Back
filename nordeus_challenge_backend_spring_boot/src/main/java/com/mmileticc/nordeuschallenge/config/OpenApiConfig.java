package com.mmileticc.nordeuschallenge.config;

import io.swagger.v3.oas.models.Components;
import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Contact;
import io.swagger.v3.oas.models.info.Info;
import io.swagger.v3.oas.models.info.License;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class OpenApiConfig {

    @Bean
    public OpenAPI apiInfo() {
        return new OpenAPI()
                .components(new Components())
                .info(new Info()
                        .title("Nordeus Challenge - Gauntlet of the Hero API")
                        .description("Clean Architecture Spring Boot backend for RPG battles")
                        .version("1.0.0")
                        .contact(new Contact().name("Nordeus Challenge"))
                        .license(new License().name("Proprietary")));
    }
}

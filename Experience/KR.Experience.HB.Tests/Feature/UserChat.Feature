@User_Chat
Feature: User chat

Scenario: User posts a chat question
    Given a valid question model "acc_200_pin_req"
    When performing a vector search and llm format operation 
    Then api returns staus code 200
    And  a data response "acc_200_pin_res"



 





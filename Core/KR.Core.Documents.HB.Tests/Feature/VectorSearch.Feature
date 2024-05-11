@Vector_Search
Feature: Vector Search

Scenario: For a given embedding search request
    Given a valid embedded search model
    When performing a vector search operation
    Then data store returns list of similar embeddings
    And  a data response "vector_doc_res"



 





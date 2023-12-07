# Mover Candidate Test

## Introduktion

Dear candidate.

Welcome to the Mover Candidate Test.
Please use you preferred way of working, and show us what you can :)

Enjoy!

## General

1. Should be able to run with a push of f5 without any installations (NuGet packages are allowed)
2. Include tests
3. Implement in-memory storage of own choice

## Tasks

1. Implement API endpoint: Calculate least angle between watch hands.
   1. Return least angle as output

2. Inventory management
   1. Create APIs to
      1. Create inventory items
         1. With SKU, description and quantity
         2. Adding inventory with unknown SKU, it should create new.
         3. Adding inventory with known SKU, it should add the quantity to existing
      2. Remove a defined quantity for a specific SKU
      3. List of all inventory

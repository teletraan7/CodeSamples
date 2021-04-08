// Copyright @2021 Jonathon Neal

#pragma once

#include "CoreMinimal.h"
#include "InteractableActorBase.h"
#include "DoubleCross/ItemStruct.h"
#include "StealableActorBase.generated.h"


UCLASS()
/// <summary>
/// Class for all stealable items that contains any data exclusive to them
/// </summary>
class DOUBLECROSS_API AStealableActorBase : public AInteractableActorBase
{
	GENERATED_BODY()

public:
	AStealableActorBase();
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = "Item Data")
	FItemStruct ItemInfo;
};
// Copyright @2021 Jonathon Neal


#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "DoubleCross/StealableActorBase.h"
#include "DoubleCross/ItemStruct.h"
#include "ItemHolderComponent.generated.h"

UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class DOUBLECROSS_API UItemHolderComponent : public UActorComponent
{
	GENERATED_BODY()

public:
	/// <summary>
	/// Gets the ItemStruct from the stealable object and stores a copy of it here, then deletes the original actor
	/// </summary>
	/// <param name="ItemBeingAdded">Reference to the interactable actor being added to this list</param>
	UFUNCTION(Server, Reliable)
	void ServerAddItem(AInteractableActorBase* ItemBeingAdded);
	void ServerAddItem_Implementation(AInteractableActorBase* ItemBeingAdded);
	void RemoveItemsFromHolder();
	UFUNCTION(Server, Reliable)
	void ServerInventoryDebug();
	void ServerInventoryDebug_Implementation();
	TArray<FItemStruct>* GetItemsHeld();
private:
	//This array contains the item structs being held by the player
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Items", meta = (AllowPrivateAccess = true));
	TArray<FItemStruct> ItemsBeingHeld;
};
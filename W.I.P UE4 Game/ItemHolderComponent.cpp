// Copyright @2021 Jonathon Neal

#include "ItemHolderComponent.h"

TArray<FItemStruct>* UItemHolderComponent::GetItemsHeld()
{
    return &ItemsBeingHeld;
}

void UItemHolderComponent::ServerAddItem_Implementation(AInteractableActorBase* ItemBeingAdded)
{
	if (GetOwnerRole() == ROLE_Authority)
    {
        //UE_LOG(LogTemp, Warning, TEXT("SERVER PICKED UP ITEM"));
	    AStealableActorBase* ItemActor = Cast<AStealableActorBase>(ItemBeingAdded);
	    ItemsBeingHeld.Add(ItemActor->ItemInfo);
	    ItemBeingAdded->Destroy();
	    //UE_LOG(LogTemp, Error, TEXT("THERE ARE %i ITEMS IN THE PLAYER ITEM HOLDER"), ItemsBeingHeld.Num());
        if (GetNetMode() == ENetMode::NM_ListenServer) UE_LOG(LogTemp, Error, TEXT("THE ITEMHOLDER ON THE SERVER IS OWNED BY %s"), *FString(GetOwner()->GetName()));
        if (GetNetMode() == ENetMode::NM_Client) UE_LOG(LogTemp, Error, TEXT("THE ITEMHOLDER ON THE CLIENT IS OWNED BY %s"), *FString(GetOwner()->GetName()));
    }
    else UE_LOG(LogTemp, Warning, TEXT("ITEM COMPONENT OWNER DOES NOT HAVE AUTHORITY"));
}

void UItemHolderComponent::ServerInventoryDebug_Implementation()
{
        if (GetNetMode() == ENetMode::NM_ListenServer) UE_LOG(LogTemp, Error, TEXT("THE ITEMHOLDER ON THE SERVER IS OWNED BY %s AND HAS %i ITEMS IN THE HOLDER"), *FString(GetOwner()->GetName()), ItemsBeingHeld.Num());
        if (GetNetMode() == ENetMode::NM_Client) UE_LOG(LogTemp, Error, TEXT("THE ITEMHOLDER ON THE SERVER IS OWNED BY %s AND HAS %i ITEMS IN THE HOLDER"), *FString(GetOwner()->GetName()), ItemsBeingHeld.Num());
}

void UItemHolderComponent::RemoveItemsFromHolder()
{
	ItemsBeingHeld.Empty();
}
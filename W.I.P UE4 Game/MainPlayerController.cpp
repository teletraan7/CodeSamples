// Copyright @2021 Jonathon Neal


#include "MainPlayerController.h"
#include "DoubleCross/ItemDropOffPoint.h"

AMainPlayerController::AMainPlayerController()
{
	ItemHolderComp = CreateDefaultSubobject<UItemHolderComponent>(TEXT("Item Holder"));	
}

void AMainPlayerController::SetupInputComponent()
{
    Super::SetupInputComponent();
    //Action Inputs
    //InputComponent->BindAction("Attack", IE_Pressed , this, &AMainPlayerController::HandleAttackInput);
    //InputComponent->BindAction("Interact", IE_Pressed , this, &AMainPlayerController::HandleInteractInput);
    InputComponent->BindAction("Hack", IE_Pressed , this, &AMainPlayerController::HandleHackInputPress);
    InputComponent->BindAction("Hack", IE_Released , this, &AMainPlayerController::HandleHackInputRelease);
}

void AMainPlayerController::SetPlayerEnabledState(bool SetPlayerEnabled)
{
    if (SetPlayerEnabled)
    {
        GetPawn()->EnableInput(this);
    }
    else
    {
        GetPawn()->DisableInput(this);
    }

    bShowMouseCursor = !SetPlayerEnabled;
    SetShowMouseCursor(bShowMouseCursor);
}

bool AMainPlayerController::ServerItemPickup_Validate(AInteractableActorBase* ItemBeingPickedUp)
{
    //Currently just return true until you figure out the best way to get the apporpriate PCharacter on the server
    if (ItemBeingPickedUp != nullptr) return true;
    else return false;
}

void AMainPlayerController::ServerItemPickup_Implementation(AInteractableActorBase* ItemBeingPickedUp)
{
    if (GetNetMode() == ENetMode::NM_ListenServer) UE_LOG(LogTemp, Error, TEXT("PLAYER CON PICKUPITEM IS BEING DONE ON THE SERVER"));
    ItemHolderComp->ServerAddItem(ItemBeingPickedUp);
}

void AMainPlayerController::ServerItemDropOff_Implementation(AItemDropOffPoint* DropPoint)
{
    if (HasAuthority())
    {
        UE_LOG(LogTemp, Error, TEXT("THERE ARE %i ITEMS IN THE PCON"), GetItemHolder()->GetItemsHeld()->Num()); 
        if (GetNetMode() == ENetMode::NM_ListenServer) UE_LOG(LogTemp, Error, TEXT("THE ITEMHOLDER ON THE SERVER IS OWNED BY %s"), *FString(ItemHolderComp->GetOwner()->GetName()));
        if (GetNetMode() == ENetMode::NM_Client) UE_LOG(LogTemp, Error, TEXT("THE ITEMHOLDER ON THE CLIENT IS OWNED BY %s"), *FString(ItemHolderComp->GetOwner()->GetName()));
        DropPoint->CollectItems(GetItemHolder());
    }
}

#pragma region INPUT METHODS
void AMainPlayerController::HandleAttackInput()
{
    //if (PCharacter) PCharacter->Attack();
}

void AMainPlayerController::HandleInteractInput()
{
    if (PCharacter) PCharacter->Interact();
}

void AMainPlayerController::HandleHackInputPress()
{
    HackPressed.Broadcast();
}

void AMainPlayerController::HandleHackInputRelease()
{
    HackReleased.Broadcast();
}
#pragma endregion 
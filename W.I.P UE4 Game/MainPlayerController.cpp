// Copyright @2020 Jonathon Neal


#include "MainPlayerController.h"

AMainPlayerController::AMainPlayerController()
{
	ItemHolderComp = CreateDefaultSubobject<UItemHolderComponent>(TEXT("Item Holder"));	
}

void AMainPlayerController::SetupInputComponent()
{
    Super::SetupInputComponent();
    //Action Inputs
    InputComponent->BindAction("Attack", IE_Pressed , this, &AMainPlayerController::HandleAttackInput);
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

void AMainPlayerController::PickUpItem(AInteractableActorBase* ItemBeingPickUp)
{
    //ASK THE SERVER TO PICK UP THE ITEM, AND IF SO THEN REPLICATE THE SERVERS VERSION OF THE ITEM HOLDER COMP TO THIS LOCAL CLIENT
    UE_LOG(LogTemp, Warning, TEXT("PLAYER CONTROLLER ASKED TO PICK UP ITEM."));
    ItemHolderComp->ServerAddItem(ItemBeingPickUp);
}

#pragma region INPUT METHODS
void AMainPlayerController::HandleAttackInput()
{
    if (PCharacter) PCharacter->Attack();
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
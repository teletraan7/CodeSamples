// Copyright @2021 Jonathon Neal


#include "PlayerCharacter.h"
#include "DoubleCross/SecurityBarrierBase.h"
#include "Net/UnrealNetwork.h"
#include "Kismet/GameplayStatics.h"
#include "Components/ArrowComponent.h"
#include "Components/CapsuleComponent.h"

#pragma region CORE UE4 METHODS
// Sets default values
APlayerCharacter::APlayerCharacter()
{
 	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = false;
	GetArrowComponent()->DestroyComponent();
	GetMesh()->DestroyComponent();
	SpriteComp = CreateDefaultSubobject<UPaperFlipbookComponent>(TEXT("Pawn Sprite"));
	SpriteComp->SetupAttachment(RootComponent);
	SpringComp = CreateDefaultSubobject<USpringArmComponent>(TEXT("Camera Spring Arm"));
	SpringComp->SetupAttachment(RootComponent);
	CameraComp = CreateDefaultSubobject<UCameraComponent>(TEXT("Player Camera"));
	CameraComp->SetupAttachment(SpringComp);
	BagMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Item Bag Mesh"));
	BagMesh->SetupAttachment(SpriteComp);
}

// Called when the game starts or when spawned
void APlayerCharacter::BeginPlay()
{
	Super::BeginPlay();
	//NOTE: This may need to be changed later. I think that currently every proxy character spawned in the server is getting the first player controller, and not the one they should use.
	PlayerCon = Cast<AMainPlayerController>(GetWorld()->GetFirstPlayerController());
	GetCapsuleComponent()->OnComponentBeginOverlap.AddDynamic(this, &APlayerCharacter::OnOverlapBegin);
	GetCapsuleComponent()->OnComponentEndOverlap.AddDynamic(this, &APlayerCharacter::OnOverlapEnd);
}

// Called to bind functionality to input
void APlayerCharacter::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);
	InputComponent->BindAxis("Horizontal Movement", this, &APlayerCharacter::MoveHorizontal);
	InputComponent->BindAxis("Vertical Movement", this, &APlayerCharacter::MoveVertical);
	InputComponent->BindAction("Interact", IE_Pressed , this, &APlayerCharacter::Interact);
	InputComponent->BindAction("Attack", IE_Pressed , this, &APlayerCharacter::Attack);
}

void APlayerCharacter::GetOwnedGameplayTags(FGameplayTagContainer& TagContainer) const
{
	UE_LOG(LogTemp, Error, TEXT("GetOwnedGameplayTags"));
}
#pragma endregion

#pragma region VARIABLE REPLICATION
void APlayerCharacter::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
{
	Super::GetLifetimeReplicatedProps(OutLifetimeProps);
	DOREPLIFETIME(APlayerCharacter, IsFacingRight);
	DOREPLIFETIME(APlayerCharacter, IsPlayerIdle);
}

void APlayerCharacter::OnRep_IsFacingRight()
{
	if (IsFacingRight) SpriteComp->AddLocalRotation(FRotator(0,180,0));
	else if (!IsFacingRight) SpriteComp->AddLocalRotation(FRotator(0,-180,0));
}

void APlayerCharacter::OnRep_IsPlayerIdle()
{
	UPaperFlipbook* ChosenAnimation = (IsPlayerIdle) ? IdleAnimation : WalkAnimation;
	if (SpriteComp->GetFlipbook() != ChosenAnimation) SpriteComp->SetFlipbook(ChosenAnimation);
}
#pragma endregion 

#pragma region DELEGATES
void APlayerCharacter::OnOverlapBegin(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	AInteractableActorBase* InteractableActor = Cast<AInteractableActorBase>(OtherActor);
	if (InteractableActor == nullptr) return;
	if (InteractableActor->InteractionType == EInteractionType::Stealable) StoreNearbyObject(InteractableActor, true);
	else if (InteractableActor->InteractionType == EInteractionType::Security) StoreSecurityRef(InteractableActor);
}

void APlayerCharacter::OnOverlapEnd(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex)
{
	AInteractableActorBase* InteractableActor = Cast<AInteractableActorBase>(OtherActor);
	if (InteractableActor == nullptr) return;
	if (InteractableActor->InteractionType == EInteractionType::Stealable) StoreNearbyObject(InteractableActor, false);
	else if (InteractableActor->InteractionType == EInteractionType::Security) StoreSecurityRef(InteractableActor);
}
#pragma endregion 

#pragma region HELPER METHODS
void APlayerCharacter::StoreSecurityRef(AInteractableActorBase* SecurityObj)
{
	if (NearbySecurityObject == nullptr) NearbySecurityObject = Cast<ASecurityBarrierBase>(SecurityObj);
	else NearbySecurityObject = nullptr;
}

void APlayerCharacter::StoreNearbyObject(AInteractableActorBase* NearbyObject, bool bAddObj)
{
	if (bAddObj && !StolenItemsNearPawn.Contains(NearbyObject)) StolenItemsNearPawn.Add(NearbyObject);
	else if (!bAddObj && StolenItemsNearPawn.Contains(NearbyObject)) StolenItemsNearPawn.Remove(NearbyObject);
	else UE_LOG(LogTemp, Warning, TEXT("INVALID PARAMATERS GIVEN TO APlayerCharacter::StoreNearbyObject"));
}
#pragma endregion

#pragma region MOVEMENT
void APlayerCharacter::MoveHorizontal(float value)
{
	if (!bIgnoreMoveInput && value != 0.0f)
	{
		if (value != 0.0f) FlipSprite(value);
		const FRotator Rotation = Controller->GetControlRotation();
		const FVector Direction = FRotationMatrix(Rotation).GetScaledAxis(EAxis::X);
		AddMovementInput(Direction, value);
	}
	UpdateAnimation();
}

void APlayerCharacter::MoveVertical(float value)
{
	if (!bIgnoreMoveInput && value != 0.0f)
	{
		const FRotator Rotation = Controller->GetControlRotation();
		const FVector Direction = FRotationMatrix(Rotation).GetScaledAxis(EAxis::Y);
		AddMovementInput(Direction, value);
	}
	UpdateAnimation();
}
#pragma endregion

#pragma region ANIMATION
void APlayerCharacter::UpdateAnimation_Implementation()
{
	IsPlayerIdle = GetVelocity().Size2D() == 0.0f;
	OnRep_IsPlayerIdle();
}

void APlayerCharacter::FlipSprite_Implementation(float InputValue)
{
	if (InputValue > 0.0f && IsFacingRight)
	{
		IsFacingRight = false;
		OnRep_IsFacingRight();
	}
	else if (InputValue < 0.0f && !IsFacingRight)
	{
		IsFacingRight = true;
		OnRep_IsFacingRight();
	}
}
#pragma endregion

#pragma region INTERACTIONS
void APlayerCharacter::Interact()
{
	UE_LOG(LogTemp, Warning, TEXT("INTERACT BUTTON PRESSED"));
	if (PlayerCon == nullptr) return;
	if (StolenItemsNearPawn.Num() > 0) PlayerCon->ServerItemPickup(StolenItemsNearPawn.Last());
	else if (StolenItemsNearPawn.Num() <= 0 && NearbySecurityObject != nullptr) NearbySecurityObject->HackSecurityBarrier(this);
	else UE_LOG(LogTemp, Error, TEXT("NO VALID PLAYER INTERACTION AVAILABLE"));
}

void APlayerCharacter::Attack()
{
	//PlayerCon->TestPickup();
	//Server_Debug();
}
#pragma endregion
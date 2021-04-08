// Copyright @2021 Jonathon Neal


#include "ItemDropOffPoint.h"
#include "DoubleCross/ItemHolderComponent.h"
#include "DoubleCross/MainPlayerController.h"

AItemDropOffPoint::AItemDropOffPoint()
{
	PrimaryActorTick.bCanEverTick = false;
	ActorMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Actor Mesh"));
	RootComponent = ActorMesh;
	ActorTriggerBox = CreateDefaultSubobject<UBoxComponent>(TEXT("Actor Trigger Box"));
	ActorTriggerBox->SetupAttachment(RootComponent);
	ActorTriggerBox->OnComponentBeginOverlap.AddDynamic(this, &AItemDropOffPoint::OnOverlapBegin);
}

void AItemDropOffPoint::BeginPlay()
{
	Super::BeginPlay();
	//PROBABALY CAN BE DONE CLEANER BUT GOOD FOR NOW
	PlayerClass = GetWorld()->GetFirstPlayerController()->GetPawn();
}

void AItemDropOffPoint::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
{
	Super::GetLifetimeReplicatedProps(OutLifetimeProps);
	DOREPLIFETIME(AItemDropOffPoint, ItemsCollected);
}

void AItemDropOffPoint::OnOverlapBegin(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if (OtherActor != PlayerClass) return;
	APlayerCharacter* Player = Cast<APlayerCharacter>(OtherActor);
	if (Player == nullptr) return;
	AlertPlayerController(Player);
}

void AItemDropOffPoint::AlertPlayerController(APlayerCharacter* Player)
{
	AMainPlayerController* PlayerCon = Player->GetPlayerCon();
	if (PlayerCon == nullptr) return;
	PlayerCon->ServerItemDropOff(this);
	UE_LOG(LogTemp, Error, TEXT("PLAYER CONTROLLER WAS TOLD TO CALL THE DROP OFF RPC"));
}

void AItemDropOffPoint::CollectItems(UItemHolderComponent* PlayerItemHolder)
{
	if (HasAuthority())
	{
		TArray<FItemStruct>* TempArray = PlayerItemHolder->GetItemsHeld();
		//NOTE: OKAY NEED TO WORK ON THE ITEM HOLDER. IT LOOKS LIKE THE INFO ISN'T MAKING IT TO THE SERVER SIDE VERSION FOR EACH PCON
		if (TempArray->Num() <= 0)
		{
			UE_LOG(LogTemp, Warning, TEXT("SERVER ITEM HOLDER IS EMPTY"));
			return;
		}
		const int iArraySize = TempArray->Num();
		for (int i = 0; i < iArraySize; i++)
		{
			FItemStruct ItemAtIndex = (*TempArray)[i];
			ItemsCollected.Add(ItemAtIndex);
			UE_LOG(LogTemp, Error, TEXT("THERE ARE %i ITEMS IN THE DROP OFF POINT"), ItemsCollected.Num());
		}
		//OKAY THIS SHOULD PROPBABLY BE A REP NOTIFY... OR A CLIENT RPC. DEPENDS ON IF REP NOTIFY IS CALLED TO ALL CLIENTS
		PlayerItemHolder->RemoveItemsFromHolder();
		UE_LOG(LogTemp, Error, TEXT("ITEM WAS ADDED AN REMOVED FROM PLAYER."));
	}
}
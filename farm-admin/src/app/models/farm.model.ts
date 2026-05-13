export interface Farm {
    id: string;
    name: string;
    location: string;
    description: string;
    isActive: boolean;
    ownerName: string;
    // Bank info — receiving account for marketplace sales + investor transfers
    bankName?: string;
    bankAccountNumber?: string;
    bankAccountHolder?: string;
    bankBranch?: string;
}
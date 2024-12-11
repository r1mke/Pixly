import { UserDTO } from "./UserDTO";

export interface PhotoGetAllResult {
    Photos: PhotoGetAllDTO[];
    TotalPhotos: number;
    TotalPages: number;
    PageNumber: number;
    PageSize: number
}

export interface PhotoGetAllDTO {
    Id: number;
    User :UserDTO;
    Approved: boolean;
    Url? : string;
    IsLiked: boolean
    LikeCount: number;
    ViewCount: number;
    CreateAt: Date
}



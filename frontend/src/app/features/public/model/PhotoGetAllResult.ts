import { PhotoDTO } from "./PhotoDTO";
import { UserDTO } from "./UserDTO";

export interface PhotoGetAllResult {
    Photos: PhotoDTO[];
    TotalPhotos: number;
    TotalPages: number;
    PageNumber: number;
    PageSize: number
}





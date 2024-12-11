import { PhotoDTO } from "./PhotoDTO";

export interface PhotoGetAllResult {
    Photos: PhotoDTO[];
    TotalPhotos: number;
    TotalPages: number;
    PageNumber: number;
    PageSize: number
}
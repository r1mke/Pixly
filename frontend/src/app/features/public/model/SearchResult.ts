import { PhotoDTO } from "./PhotoDTO";


export interface SearchResult {
    Photos: PhotoDTO[];
    TotalPhotos: number;
    TotalPages: number;
    PageNumber: number;
    PageSize: number
}
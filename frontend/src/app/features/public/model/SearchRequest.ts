export interface SearchRequest {
    Popularity:string;
    Title:string;
    Orientation:string | null;
    Size:string | null;
    Color: string | null;
    PageNumber: number;
    PageSize: number;
}
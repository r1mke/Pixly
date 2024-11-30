import { Category } from "./category";

export interface PostPhoto {
    Title:string;
    Description:string;
    Location:string;
    UserId : number;
    Files : File;
    Tags : string[];
    Categories : Category[];    
  }
  
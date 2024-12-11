export interface PhotoDTO {
    Id: number;
    Title: string;
    Description: string;
    LikeCount: number;
    ViewCount: number;
    Price: number;
    Location: string;
    User?: User;
    Approved: boolean;
    CreateAt: Date;
    Orientation?: string;
    Url: string;
    Size?: string;
    IsLiked: boolean;
  }
  

export interface User {
    Id: number;
    Username: string;
    FirstName: string;
    LastName: string;
    Email: string;
    ProfilePicture?: string;
  }
  
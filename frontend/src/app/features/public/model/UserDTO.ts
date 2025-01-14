export interface UserDTO{
    Id: number;
    FirstName: string;
    LastName: string;
    Username: string;
    Email: string;
    ProfileImgUrl? :string;
    IsAdmin: boolean;
}
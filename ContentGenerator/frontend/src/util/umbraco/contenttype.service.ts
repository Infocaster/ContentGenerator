export interface IContentTypeService {
    getAllowedTypes(contentTypeId: number): Promise<IContentType[]>;
}

export interface IContentType {
    id: number;
    icon: string;
    name: string;
    alias: string;
}
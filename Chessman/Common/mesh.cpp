/* Copyright © 2014
AUTHOR      : Vlad Varna
DESCRIPTION : Generic mesh handler with position, normals, 2D texture and color
*/
#include "DXUT.h"
#include "SDKmisc.h"
#include "mesh.h"

#include <algorithm>

#pragma comment(lib, "lib3ds.lib")  

//float M_null[16]={1.f,0.f,0.f,0.f,0.f,1.f,0.f,0.f,0.f,0.f,1.f,0.f,0.f,0.f,0.f,1.f};

void MeshVNTC::Init(NAT nrv,NAT ov,NAT on,NAT ot,NAT oc,NAT tip)
    {
	stride=ov; oN=on; oT=ot; oC=oc;
	Type=tip;
    vb.clear();
	if(nrv)
		vb.reserve(nrv);
	}

void MeshVNTC::Free()
	{
	vb.clear();
    }


void MeshVNTC::EndObject()
    {
    NAT o=ob.size();
    if(o>0)
        ob[o-1].stop=vb.size()-1; // end last object
    }

NAT MeshVNTC::NewObject(const TCHAR* name,NAT color,NAT texture)
    {
    EndObject();
    NAT o=ob.size();
    ob.resize(o+1);
    ob[o].start=vb.size();
    if(name)
        _tcscpy_s(ob[o].name,name);
    ob[o].cbPS=color;
    ob[o].texture=texture;
//    ob[o].stop=-1;
    return o;
    }


NAT MeshVNTC::NewObjectA(const char* name,NAT color,NAT texture)
    {
    NAT o=NewObject(NULL,color,texture);
    _stprintf_s(ob[o].name,_T("%S"),name);
    return o;
    }


NAT MeshVNTC::GetObjectIndex(const TCHAR*name,NAT colorIndex)
    {
    for(int oi=0;oi<ob.size();oi++)
        {
        if(0==_tcscmp(ob[oi].name,name))
            if(-1==colorIndex || ob[oi].cbPS==colorIndex)
                return oi;
        }
    LOG(ERROR) << "Unknown mesh object " << name;
    return 0; //TODO: maybe create an empty object
    }


UOBJECT& MeshVNTC::GetObject(const TCHAR*name,NAT colorIndex)
    {
    return ob[GetObjectIndex(name,colorIndex)];
    }


void MeshVNTC::Vertex(NAT vi,float x,float y,float z,float nx,float ny,float nz, float u, float v, DWORD c)
    {
    if(vi>=vb.size())
        {
        if(vi>=vb.capacity())
            vb.reserve(vi*2); // grow to allow another triangle
        vb.resize(vi+1);
        }
    vb[vi].Vert(x,y,z);
    vb[vi].Norm(nx,ny,nz);
    vb[vi].Tex(u,v);
    vb[vi].Col(c);
    }


#define X_AXIS 1
#define Y_AXIS 2
#define Z_AXIS 3

// 1=X, 2=Y, 3=Z
int DominantAxisInNormal(int&s,float x,float y,float z)
    {
    float max=abs(x);
    s=x<0?-1:1;
    int axis=X_AXIS;
    if(abs(y)>max)
        {
        max=abs(y);
        s=y<0?-1:1;
        axis=Y_AXIS;
        } 
    if(abs(z)>max)
        {
        max=abs(z);
        s=z<0?-1:1;
        axis=Z_AXIS;
        } 
    return axis;
    }


void MeshVNTC::TriangleFlat(NAT ti,float x1,float y1,float z1,
                                   float x2,float y2,float z2,
                                   float x3,float y3,float z3,
                                   DWORD c, bool invN)
    {
    NAT vi=ti*3;
    if(vi>=vb.size())
        {
        if(vi>=vb.capacity())
            vb.reserve(vi*2); // grow to allow another triangle
        vb.resize(vi+3);
        }
    vb[vi].Vert(x1,y1,z1);
    vb[vi+1].Vert(x2,y2,z2);
    vb[vi+2].Vert(x3,y3,z3);
    
    //Vcross(N,A[0]-C[0],A[1]-C[1],A[2]-C[2],B[0]-C[0],B[1]-C[1],B[2]-C[2]);
    XMVECTOR xFaceNormal=XMVector3Cross(XMVectorSet(x1-x3,y1-y3,z1-z3,1),XMVectorSet(x2-x3,y2-y3,z2-z3,1));
    xFaceNormal=XMVector3Normalize(xFaceNormal);
    if(invN)
        xFaceNormal=-xFaceNormal;
    XMFLOAT3 f3FaceNorm;
    XMStoreFloat3(&f3FaceNorm,xFaceNormal);

    vb[vi].Norm(f3FaceNorm);
    vb[vi+1].Norm(f3FaceNorm);
    vb[vi+2].Norm(f3FaceNorm);

    int sign=0;
    int axis=DominantAxisInNormal(sign,f3FaceNorm.x,f3FaceNorm.y,f3FaceNorm.z);
    sign=-sign;
    if(X_AXIS==axis)
        {
        vb[vi].Tex(z1*sign,y1,Uscale,Vscale,Ushift,Vshift);
        vb[vi+1].Tex(z2*sign,y2,Uscale,Vscale,Ushift,Vshift);
        vb[vi+2].Tex(z3*sign,y3,Uscale,Vscale,Ushift,Vshift);
        }
    else if(Y_AXIS==axis)
        {
        vb[vi].Tex(x1*sign,z1,Uscale,Vscale,Ushift,Vshift);
        vb[vi+1].Tex(x2*sign,z2,Uscale,Vscale,Ushift,Vshift);
        vb[vi+2].Tex(x3*sign,z3,Uscale,Vscale,Ushift,Vshift);
        }
    else if(Z_AXIS==axis)
        {
        vb[vi].Tex(x1*sign,y1,Uscale,Vscale,Ushift,Vshift);
        vb[vi+1].Tex(x2*sign,y2,Uscale,Vscale,Ushift,Vshift);
        vb[vi+2].Tex(x3*sign,y3,Uscale,Vscale,Ushift,Vshift);
        }
    
    vb[vi].Col(c);
    vb[vi+1].Col(c);
    vb[vi+2].Col(c);
    }


// origin  is middle of upper hole in LH, dy is height and is down
void MeshVNTC::Band(NAT&ti,float x0,float y0,float z0, float dx1,float dz1, float dx2,float dz2, float dy,DWORD c, bool invN)
    {
    //front (-dz)
    TriangleFlat(ti++, x0+dx1,y0+dy,z0-dz1, x0-dx1,y0+dy,z0-dz1, x0+dx2,y0,z0-dz2, c,invN);
    TriangleFlat(ti++, x0-dx1,y0+dy,z0-dz1, x0-dx2,y0,z0-dz2, x0+dx2,y0,z0-dz2, c,invN);

    //back (+dz)
    TriangleFlat(ti++, x0+dx1,y0+dy,z0+dz1, x0-dx1,y0+dy,z0+dz1, x0+dx2,y0,z0+dz2, c,!invN);
    TriangleFlat(ti++, x0-dx1,y0+dy,z0+dz1, x0-dx2,y0,z0+dz2, x0+dx2,y0,z0+dz2, c,!invN);

    //right (+dx)
    TriangleFlat(ti++, x0+dx1,y0+dy,z0+dz1, x0+dx1,y0+dy,z0-dz1, x0+dx2,y0,z0+dz2, c,invN);
    TriangleFlat(ti++, x0+dx1,y0+dy,z0-dz1, x0+dx2,y0,z0-dz2, x0+dx2,y0,z0+dz2, c,invN);

    //left (-dx)
    TriangleFlat(ti++, x0-dx1,y0+dy,z0+dz1, x0-dx1,y0+dy,z0-dz1, x0-dx2,y0,z0+dz2, c,!invN);
    TriangleFlat(ti++, x0-dx1,y0+dy,z0-dz1, x0-dx2,y0,z0-dz2, x0-dx2,y0,z0+dz2, c,!invN);
    }


// origin  is middle of upper hole in LH, height is down, r1 and r3 define the lower face, r2 and r4 the upper
void MeshVNTC::CubeHole(NAT&ti,float x0,float y0,float z0, float r1,float r2,float r3,float r4, float h,DWORD c)
    {
    _ASSERTE(r3>=r1);
    _ASSERTE(r4>=r2);
    // lower face
    Band(ti, x0,y0+h,z0, r1,r1,r3,r3, 0,c);
    // upper face
    Band(ti, x0,y0,z0, r4,r4,r2,r2, 0,c);
    
    // outside
    Band(ti, x0,y0,z0, r3,r3,r4,r4, h,c);

    // inside
    Band(ti, x0,y0,z0, r1,r1,r2,r2, h,c);
    }


// origin  is (left,down,near) in LH 
void MeshVNTC::Cube(NAT&ti,float x0,float y0,float z0,float R,DWORD c)
    {
    float xR=x0+R,yR=y0+R,zR=z0+R;
    //2*6*3=36 vertices
    Uscale=Vscale=1./R;
    Vscale=-Vscale; //Y is up, not down

    //y const top
    TriangleFlat(ti++,x0,yR,z0,x0,yR,zR,xR,yR,z0,c);
    TriangleFlat(ti++,x0,yR,zR,xR,yR,zR,xR,yR,z0,c);

    //y const bottom
    TriangleFlat(ti++,x0,y0,zR,x0,y0,z0,xR,y0,z0,c);
    TriangleFlat(ti++,xR,y0,zR,x0,y0,zR,xR,y0,z0,c);

    //x const left
    TriangleFlat(ti++,x0,yR,z0,x0,y0,z0,x0,y0,zR,c);
    TriangleFlat(ti++,x0,yR,zR,x0,yR,z0,x0,y0,zR,c);

    //x const right
    TriangleFlat(ti++,xR,y0,z0,xR,yR,z0,xR,y0,zR,c);
    TriangleFlat(ti++,xR,yR,z0,xR,yR,zR,xR,y0,zR,c);

    //z const near
    TriangleFlat(ti++,x0,y0,z0,x0,yR,z0,xR,y0,z0,c);
    TriangleFlat(ti++,x0,yR,z0,xR,yR,z0,xR,y0,z0,c);

    //z const far
    TriangleFlat(ti++,x0,yR,zR,x0,y0,zR,xR,y0,zR,c);
    TriangleFlat(ti++,xR,yR,zR,x0,yR,zR,xR,y0,zR,c);

    }


// origin  is (left,down,near) in LH 
void MeshVNTC::CubeCorner(NAT&ti,float x0,float y0,float z0,float R,DWORD cX,DWORD cY,DWORD cZ)
    {
    float xR=x0+R,yR=y0+R,zR=z0+R;
    //2*6*3=36 vertices
    Uscale=Vscale=-1./R;
    Vscale=-Vscale; //Y is up, not down

    //x const left
    TriangleFlat(ti++,x0,yR,z0,x0,y0,z0,x0,y0,zR,cX);
    
    //y const bottom
    TriangleFlat(ti++,x0,y0,zR,x0,y0,z0,xR,y0,z0,cY);
    
    //z const near
    TriangleFlat(ti++,x0,y0,z0,x0,yR,z0,xR,y0,z0,cZ);
    }


void MeshVNTC::RhombicBipyramid(NAT&ti,float x0,float y0,float z0,float Rx,float Ry,float Rz,DWORD c)
    {
    float xR=x0+Rx,yR=y0+Ry,zR=z0+Rz;
    //2*6*3=36 vertices
    Uscale=Vscale=-1./std::max(Rx,std::max(Ry,Rz));
    Vscale=-Vscale; //Y is up, not down

    //quadrant I
    TriangleFlat(ti++,0,yR,0,0,0,zR,xR,0,0,c);
    TriangleFlat(ti++,0,-yR,0,xR,0,0,0,0,zR,c);
    
    //quadrant II
    TriangleFlat(ti++,0,yR,0,-xR,0,0,0,0,zR,c);
    TriangleFlat(ti++,0,-yR,0,0,0,zR,-xR,0,0,c);
    
    //quadrant III
    TriangleFlat(ti++,0,yR,0,0,0,-zR,-xR,0,0,c);
    TriangleFlat(ti++,0,-yR,0,-xR,0,0,0,0,-zR,c);

    //quadrant IV
    TriangleFlat(ti++,0,yR,0,xR,0,0,0,0,-zR,c);
    TriangleFlat(ti++,0,-yR,0,0,0,-zR,xR,0,0,c);
    }


/*
void VNTCMESH::Norm(NAT vi,float nx,float ny,float nz)
    {
    _ASSERTE(vi<vb.size());
    vb[vi].nx=nx;
    vb[vi].ny=ny;
    vb[vi].nz=nz;
    }

void VNTCMESH::Tex(NAT vi,float u,float v)
    {
    _ASSERTE(vi<vb.size());
    vb[vi].u=u;
    vb[vi].v=v;
    }
*/


bool MeshVNTC::Load3DSMesh(Lib3dsFile *file3ds, Lib3dsMeshInstanceNode *node)
    {
    float (*orig_vertices)[3];
    int export_texcos;
    int export_normals;
    int t, vert;
    Lib3dsMesh *mesh;
    FILE*o=NULL;
    int max_vertices = 0;
    int max_texcos = 0;
    int max_normals = 0;

    mesh = lib3ds_file_mesh_for_node(file3ds, (Lib3dsNode*)node);
    if (!mesh || !mesh->vertices) return false;

    NAT obj=NewObjectA(node->instance_name[0]? node->instance_name : node->base.name,def.cbPSind,def.texind);

    ob[obj].SetRot(-XM_PI/2,1.f,0.f,0.f); //around X
    ob[obj].Rotate(-XM_PI/2,0.f,1.f,0.f); //around Y
    ob[obj].Scale(1.3f,1.3f,1.3f);

    orig_vertices = (float(*)[3])malloc(sizeof(float) * 3 * mesh->nvertices);
    memcpy(orig_vertices, mesh->vertices, sizeof(float) * 3 * mesh->nvertices);

//*
    float inv_matrix[4][4], M[4][4];
    float tmp[3];
    int i2;

    lib3ds_matrix_copy(M, node->base.matrix);
    lib3ds_matrix_translate(M, -node->pivot[0], -node->pivot[1], -node->pivot[2]);
    lib3ds_matrix_copy(inv_matrix, mesh->matrix);
    lib3ds_matrix_inv(inv_matrix);
    lib3ds_matrix_mult(M, M, inv_matrix);

    for (i2 = 0; i2 < mesh->nvertices; ++i2)
        {
        lib3ds_vector_transform(tmp, M, mesh->vertices[i2]);
        lib3ds_vector_copy(mesh->vertices[i2], tmp);
        }
//*/

    export_texcos = (mesh->texcos != 0);
    export_normals = (mesh->faces != 0);

    //for (i = 0; i < mesh->nvertices; ++i) {
    //    fprintf(o, "v %f %f %f\n", mesh->vertices[i][0],mesh->vertices[i][1],mesh->vertices[i][2]);
    //    }
//    fprintf(o, "# %d vertices\n", mesh->nvertices);

//    if (export_texcos)
//        {
//        for (i = 0; i < mesh->nvertices; ++i)
//            {
//            fprintf(o, "vt %f %f\n", mesh->texcos[i][0],mesh->texcos[i][1]);
//            }
////        fprintf(o, "# %d texture vertices\n", mesh->nvertices);
//        }
//

    float (*normals)[3]=nullptr; //pointer to an array of float[3]

    if (export_normals)
        {
        normals = (float(*)[3])malloc(sizeof(float) * 9 * mesh->nfaces);
        lib3ds_mesh_calculate_vertex_normals(mesh, normals);
        }

    NAT vi=vb.size();
    
    // this copies indexed vertices but 
    //for (i = 0; i < mesh->nvertices; ++i)
    //    {
    //    Vertex(vi++,mesh->vertices[i][0],mesh->vertices[i][1],mesh->vertices[i][2],
    //                normals[i][0],normals[i][1],normals[i][2],
    //                mesh->texcos[i][0],mesh->texcos[i][1],
    //                BGRA_DW(file3ds->ambient[0],file3ds->ambient[1],file3ds->ambient[2],0xff));
    //    }

//*
    int mat_index = -1;
    for (t = 0; t < mesh->nfaces; ++t)
        {
        if (mat_index != mesh->faces[t].material)
            {
            mat_index = mesh->faces[t].material;
            if (mat_index != -1)
                {
                //fprintf(o, "usemtl %s\n", file3ds->materials[mat_index]->name);
                }
            }

        //fprintf(o, "f ");
        NAT vind,tind,nind;
        DWORD color=BGRA_DW(file3ds->ambient[0],file3ds->ambient[1],file3ds->ambient[2],0xff);
        float u,v;
        for (vert = 0; vert < 3; ++vert)
            {
            
            vind=mesh->faces[t].index[vert];
            tind=vind;
            nind=3 * t + vert;
            // TODO: handle missing normals or textures
            if(mesh->texcos)
                {
                u=mesh->texcos[tind][0];
                v=mesh->texcos[tind][1];
                }
            else
                u=v=0;

            Vertex(vi++,mesh->vertices[vind][0],mesh->vertices[vind][1],mesh->vertices[vind][2],
                normals[nind][0],normals[nind][1],normals[nind][2],
                u,v,color
                );
            }
        }
//*/

    //max_vertices += mesh->nvertices;
    //if (export_texcos) 
    //    max_texcos += mesh->nvertices;
    //if (export_normals) 
    //    max_normals += 3 * mesh->nfaces;

    free(normals);  

    memcpy(mesh->vertices, orig_vertices, sizeof(float) * 3 * mesh->nvertices);
    free(orig_vertices);
    EndObject();
    return true;
    }


bool MeshVNTC::Load3DSNode(Lib3dsFile *file3ds, Lib3dsNode *first_node)
    {
    Lib3dsNode *pn;
    for (pn = first_node; pn; pn = pn->next)
        {
        if (pn->type == LIB3DS_NODE_MESH_INSTANCE)
            {
            Load3DSMesh(file3ds, (Lib3dsMeshInstanceNode*)pn);
            Load3DSNode(file3ds, pn->childs);
            }
        }
    return true;
    }


BOOL MeshVNTC::Load3DS(TCHAR* path)
    {
    HRESULT hr;
    WCHAR wstr[MAX_PATH]=L"";
    DXUTFindDXSDKMediaFileCch( wstr, MAX_PATH, path );
    char str[MAX_PATH]="";
    wcstombs(str,wstr,MAX_PATH);

    Lib3dsFile * file3ds=nullptr;
    file3ds = lib3ds_file_open(str);
    //loading the model failed 
    if(!file3ds)
        {
        //LOG(DEBUG) << "Couldn't find 3DS mesh";
        return false;
        }
    if (!file3ds->nodes)
        lib3ds_file_create_nodes_for_meshes(file3ds);
    lib3ds_file_eval(file3ds, 0);
    
    return Load3DSNode(file3ds,file3ds->nodes);
    }

ID3D11Buffer* MeshVNTC::CreateVB(ID3D11Device*dev)
    {

    ID3D11Buffer*pVertexBuffer = NULL;

    if(0==vb.size())
        return NULL; //no data

    D3D11_BUFFER_DESC bd;
    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.ByteWidth = stride*sizeof(float)*vb.size();
    bd.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bd.CPUAccessFlags = 0;
    bd.MiscFlags = 0;
    bd.StructureByteStride=0;

    D3D11_SUBRESOURCE_DATA InitData;
    InitData.pSysMem = vb.data();
    HRESULT hr = dev->CreateBuffer( &bd, &InitData, &pVertexBuffer );

    if( FAILED( hr ) )
        {
        return NULL;
        }

    return pVertexBuffer;
    }

//--------------------------------------------------------------------------------------
// Given a ray origin (orig) and direction (dir), and three vertices of a triangle, this
// function returns TRUE and the interpolated texture coordinates if the ray intersects 
// the triangle
//--------------------------------------------------------------------------------------
bool IntersectTriangle( const XMVECTOR& orig, const XMVECTOR& dir,
                       XMVECTOR& v0, XMVECTOR& v1, XMVECTOR& v2,
                       FLOAT* t, FLOAT* u, FLOAT* v )
    {
    // Find vectors for two edges sharing vert0
    XMVECTOR edge1 = v1 - v0;
    XMVECTOR edge2 = v2 - v0;

    // Begin calculating determinant - also used to calculate U parameter
    XMVECTOR pvec;
    pvec= XMVector3Cross( dir, edge2 );

    // If determinant is near zero, ray lies in plane of triangle
    FLOAT det = XMVectorGetX(XMVector3Dot( edge1, pvec ));

    XMVECTOR tvec;
    if( det > 0 )
        {
        tvec = orig - v0;
        }
    else
        {
        tvec = v0 - orig;
        det = -det;
        }

    if( det < 0.0001f )
        return FALSE;

    // Calculate U parameter and test bounds
    *u = XMVectorGetX(XMVector3Dot( tvec, pvec ));
    if( *u < 0.0f || *u > det )
        return FALSE;

    // Prepare to test V parameter
    XMVECTOR qvec;
    qvec=XMVector3Cross( tvec, edge1 );

    // Calculate V parameter and test bounds
    *v = XMVectorGetX(XMVector3Dot( dir, qvec ));
    if( *v < 0.0f || *u + *v > det )
        return FALSE;

    // Calculate t, scale parameters, ray intersects triangle
    *t = XMVectorGetX(XMVector3Dot( edge2, qvec ));
    FLOAT fInvDet = 1.0f / det;
    *t *= fInvDet;
    *u *= fInvDet;
    *v *= fInvDet;

    return TRUE;
    }

bool MeshVNTC::RayIntersect(NAT ti,FXMVECTOR Origin,FXMVECTOR Direction)
    {
    NAT vi=ti*3;
    //XMFLOAT3 O,D;
    //XMStoreFloat3(&O,Origin);
    //XMStoreFloat3(&D,Origin+Direction);
    //return VinT(&O.x,&D.x,&vb[vi].x,&vb[vi+1].x,&vb[vi+2].x);
    XMVECTOR v0=XMVectorSet(vb[vi].x,vb[vi].y,vb[vi].z,1);
    vi++;
    XMVECTOR v1=XMVectorSet(vb[vi].x,vb[vi].y,vb[vi].z,1);
    vi++;
    XMVECTOR v2=XMVectorSet(vb[vi].x,vb[vi].y,vb[vi].z,1);
    FLOAT u,v,t;
    return IntersectTriangle(Origin,Direction,v0,v1,v2,&u,&v,&t);
    }

NAT MeshVNTC::RayIntersectO(NAT oi,FXMVECTOR Origin,FXMVECTOR Direction)
    {
    //XMVECTOR OriginW,DirectionW; //in world coordinates
    //OriginW = XMVector3Transform( Origin, XMMatrixInverse( nullptr, ob[oi].mw));
    //DirectionW = XMVector3Transform( Direction, XMMatrixInverse( nullptr, ob[oi].mw));

    int k=0;
    for(NAT ti=ob[oi].start/3;ti<=ob[oi].stop/3;ti++)
        {
        if(RayIntersect(ti,Origin,Direction))
            {
            return ti;
            }
        k++;
        }
    return -1;
    }

bool MeshVNTC::PointIntersect(NAT ti,float mx,float my,CXMMATRIX mWorldViewProj)
    {
    NAT vi=ti*3;
    XMVECTOR v0=vb[vi].ToViewSpace(mWorldViewProj);
    vi++;
    XMVECTOR v1=vb[vi].ToViewSpace(mWorldViewProj);
    vi++;
    XMVECTOR v2=vb[vi].ToViewSpace(mWorldViewProj);
    XMFLOAT2 A,B,C,M(mx,my);
    XMStoreFloat2(&A,v0);
    XMStoreFloat2(&B,v1);
    XMStoreFloat2(&C,v2);
    return 0!=PinT2(&M.x,&A.x,&B.x,&C.x);
    }

NAT MeshVNTC::PointIntersectO(NAT oi,float mx,float my,CXMMATRIX mViewProj)
    {
    int k=0;
    for(NAT ti=ob[oi].start/3;k<2&&ti<=ob[oi].stop/3;ti++)
        {
        if(PointIntersect(ti,mx,my,ob[oi].mrot*ob[oi].mw*mViewProj))
            {
            return ti;
            }
        k++;
        }
    return -1;
    }


/* Copyright © 2014
AUTHOR      : Vlad Varna
DESCRIPTION : Generic mesh handler with position, normals, 2D texture and color
*/

#pragma once
#include "types.h"
#include "DirectXMath.h"
#include <math.h>
#include "..\lib3DS\lib3ds.h"

using namespace DirectX;

#define BGRA_DW(r,g,b,a)	((((DWORD)a)<<24)|(((DWORD)r)<<16)|(((DWORD)g)<<8)|((DWORD)b)) //GDI
#define RGBA_DW(r,g,b,a)	((((DWORD)a)<<24)|(((DWORD)b)<<16)|(((DWORD)g)<<8)|((DWORD)r)) //DX

union UVERTEX
    {
    struct  
        {
        float x,y,z;
        float nx,ny,nz;
        float u,v;
        DWORD c;
        };
    struct
        {
        float pos[3];
        float n[3];
        float tex[2];
        float grey;
        };
    float pf[3+3+2+1];
    
    void Vert(float _x,float _y,float _z=0)
        {
        x=_x;
        y=_y;
        z=_z;
        }

    void Norm(float _x,float _y,float _z)
        {
        nx=_x;
        ny=_y;
        nz=_z;
        }

    void Tex(float _u,float _v)
        {
        u=_u;
        v=_v;
        }

    // note that origin is set before scaling, so 
    void Tex(float _u,float _v,float uS,float vS=-1.f,float uO=0.f,float vO=0.f)
        {
        u=(_u-uO)*uS;
        v=(_v-vO)*vS;
        }

    void Col(DWORD argb=0xff)
        {
        c=argb;
        }
    
    void Norm(XMVECTOR _xn)
        {
        nx=XMVectorGetX(_xn);
        ny=XMVectorGetY(_xn);
        nz=XMVectorGetZ(_xn);
        }

    void Norm(XMFLOAT3 _xn)
        {
        nx=_xn.x;
        ny=_xn.y;
        nz=_xn.z;
        }
    
    XMVECTOR ToViewSpace(CXMMATRIX mWorldViewProj)
        {
        //XMVECTOR orthov=XMVector3Transform(XMVectorSet(x,y,z,1.f),mWorldViewProj);
        return XMVector3Project(XMVectorSet(x,y,z,1.f),0.f,0.f,
                DXUTGetDXGIBackBufferSurfaceDesc()->Width,
                DXUTGetDXGIBackBufferSurfaceDesc()->Height,
                0.f,1.f,mWorldViewProj,XMMatrixIdentity(),XMMatrixIdentity());
        }

    };


//number of triangles in a list, strip or fan -----------------------------
inline unsigned NumTris(NAT nrv,int tip=1)
    {
    return (tip==1?nrv/3:nrv-2);
    }


struct UOBJECT
    {
    TCHAR name[64]; // friendly name
    XMMATRIX mw; //world position matrix
    XMMATRIX mrot; //object rotation and scale matrix
    NAT start, stop; //offsets in vertex buffer
    NAT texture; //texture index
    NAT cbPS; //color index
    DWORD highlight; //

    UOBJECT():name(),start(-1),stop(0),texture(-1),cbPS(-1),highlight(0xffffffff)
        {
        mw=XMMatrixIdentity();
        mrot=XMMatrixIdentity();
        }

    NAT Size()
        {
        _ASSERTE(stop>=start);
        return stop-start+1;
        }
    
    void SetPos(float x,float y,float z)
        {
        mw=XMMatrixTranslation(x,y,z);
        }

    void SetPos(XMVECTOR pos)
        {
        mw=XMMatrixTranslationFromVector (pos);
        }

    void Translate(float x,float y,float z)
        {
        mw*=XMMatrixTranslation(x,y,z);
        }

    void SetRot(float angleRads,float ax,float ay,float az)
        {
        mrot=XMMatrixRotationAxis(XMVectorSet(ax,ay,az,0.f),angleRads);
        }                                 
	
	void SetRot( CXMMATRIX mat )
		{
			mrot = mat;
		}                                 

    void Rotate(float angleRads,float ax,float ay,float az)
        {
        mrot*=XMMatrixRotationAxis(XMVectorSet(ax,ay,az,0.f),angleRads);
        }                                 

    void SetScale(float sx,float sy,float sz)
        {
        mrot=XMMatrixScaling(sx,sy,sz);
        }                                 

    void Scale(float sx,float sy,float sz)
        {
        mrot*=XMMatrixScaling(sx,sy,sz);
        }                                 
    };


class MeshVNTC //Vertex Normal Texture Color buffer
	{
	public:
		int Type; //Type: 0-unknown,1-list,2-strip,3-fan (RFU)
        std::vector<UVERTEX> vb;
		NAT oN,oT,oC;
        NAT stride;
        std::vector<UOBJECT> ob;
        //XMMATRIX omw; //object world matrix (UNUSED)
        
        //internal
        float Uscale,Vscale; //texture coordinates scale
        float Ushift,Vshift; //texture coordinates origin
        struct Defaults
            {
            DWORD color; //for vertex
            NAT texind,cbPSind; //for object
            }def;

        MeshVNTC():vb(),ob(),stride(0),oN(3),oT(6),oC(7),Type(1),Uscale(1.),Vscale(-1.),Ushift(0.),Vshift(0.),def() {}
		~MeshVNTC() 	{ Free(); }

		void Init(NAT nrv=3,NAT ov=(sizeof(UVERTEX)/sizeof(float)),NAT on=3,NAT ot=6,NAT oc=7,NAT tip=1);
		void Free();
		int From(TCHAR* filename=NULL,TCHAR* filetype=NULL,NAT frmnum=0);
		int To(TCHAR* filename=NULL,TCHAR* filetype=NULL,NAT frmnum=0);
		void CopyUV(int count=1);
        NAT NumVertices() {return vb.size(); }
        NAT NumTriangles() {return NumTris(vb.size(),Type); }

        // Create geometry
        NAT NewObject(const TCHAR* name,NAT color=-1,NAT texture=-1);
        NAT NewObjectA(const char* name,NAT color=-1,NAT texture=-1);
        void EndObject();
        NAT GetObjectIndex(const TCHAR*name,NAT colorIndex=-1);
        UOBJECT& GetObject(const TCHAR*name,NAT colorIndex=-1);
        
        void Vertex(NAT vi,float x,float y,float z,float nx,float ny,float nz, float u, float v, DWORD c);
        void TriangleFlat(NAT ti,float x1,float y1,float z1,float x2,float y2,float z2,float x3,float y3,float z3,DWORD c, bool invN=false);
        void Band(NAT&ti,float x0,float y0,float z0, float dx1,float dz1, float dx2,float dz2, float dy,DWORD c, bool invN=false);
        void CubeHole(NAT&ti,float x0,float y0,float z0, float r1,float r2,float r3,float r4, float h,DWORD c);
        void Cube(NAT&ti,float x0,float y0,float z0,float R,DWORD c);
        void CubeCorner(NAT&ti,float x0,float y0,float z0,float R,DWORD cX,DWORD cY,DWORD cZ);
        void RhombicBipyramid(NAT&ti,float x0,float y0,float z0,float xR,float yR,float zR,DWORD c);

        bool RayIntersect(NAT ti,FXMVECTOR Origin,FXMVECTOR Direction);
        NAT RayIntersectO(NAT oi,FXMVECTOR Origin,FXMVECTOR Direction);
        bool PointIntersect(NAT ti,float mx,float my,CXMMATRIX mWorldViewProj);
        NAT PointIntersectO(NAT oi,float mx,float my,CXMMATRIX mViewProj);

        // load external
        BOOL Load3DS(TCHAR* path);
        bool Load3DSNode(Lib3dsFile *file3ds, Lib3dsNode *first_node);
        bool Load3DSMesh(Lib3dsFile *f, Lib3dsMeshInstanceNode *node);

        // DX stuff
        ID3D11Buffer* CreateVB(ID3D11Device*);
	};


//calculeaza ecuatia unei drepte (det de 2 puncte A si B) intr-un punct P --------------------------
inline float PonL2(float *P,float *A,float *B)
    {
    return P[0]*(B[1]-A[1])+P[1]*(A[0]-B[0])+B[0]*A[1]-A[0]*B[1]; //if 0 P apartine AB
    }


//verifica daca P este in int ABC ------------------------------------------------------------------
inline int PinT2(float *P,float *A,float *B,float *C)
    {
    if(PonL2(P,A,B)>0)
        {
        if(PonL2(P,B,C)>0)
            {
            if(PonL2(P,C,A)>0) return 1;
            }
        }
    else
        {
        if(PonL2(P,B,C)<=0)
            {
            if(PonL2(P,C,A)<=0) return -1;
            }
        }
    return 0;
    }


//makes a vector a unit vector ------------------------------------------------------------
inline void Vunit(float *vct)
    {
    float l;
    l=1.0f/sqrt(vct[0]*vct[0]+vct[1]*vct[1]+vct[2]*vct[2]);
    vct[0]*=l;
    vct[1]*=l;
    vct[2]*=l;
    }


//calculates vector dot product (Vdot = A dot B) ---------------------------------------
inline float Vdot(float *A,float *B)
    {
    return (A[0]*B[0]+A[1]*B[1]+A[2]*B[2]);
    }


//calculates vector cross product (vct=AxB) -----------------------------------------------
inline void Vcross(float *vct,float Ax,float Ay,float Az,float Bx,float By,float Bz)
    {
    vct[0]=Ay*Bz-Az*By;
    vct[1]=Az*Bx-Ax*Bz;
    vct[2]=Ax*By-Ay*Bx;
    }


//calculates face normal from 3 verts -----------------------------------------------------
inline void Tnorm(float *N,float *A,float *B,float *C)
    {
    Vcross(N,A[0]-C[0],A[1]-C[1],A[2]-C[2],B[0]-C[0],B[1]-C[1],B[2]-C[2]);
    Vunit(N);
    }


//calculeaza distanta de la un punct P la un plan ABC ---------------------------------------
inline float dPP(float *P,float *A,float *B,float *C)
    {
    float N[3];
    Tnorm(N,A,B,C);
    return Vdot(P,N)-Vdot(A,N);
    }


//determines if the 3d seg OP intersects tri ABC --------------------------------------
inline int VinT(float *O,float *P,float *A,float *B,float *C)
    {
    if(dPP(P,O,A,B)>=0)
        {
        if(dPP(P,O,B,C)>=0)
            {
            if(dPP(P,O,C,A)>=0)
                {
                if(dPP(P,A,B,C)>=0)
                    {
                    if(dPP(O,A,B,C)<0) return 1;
                    }
                else
                    {
                    if(dPP(O,A,B,C)>0) return 1;
                    }
                }
            }
        } 
    else
        {
        if(dPP(P,O,B,C)<0)
            {
            if(dPP(P,O,C,A)<0)
                {
                if(dPP(P,A,B,C)>=0)
                    {
                    if(dPP(O,A,B,C)<0) return 1;
                    }
                else
                    {
                    if(dPP(O,A,B,C)>0) return 1;
                    }
                }
            }
        } 
    return 0;
    }

inline TSTR Vector2ToString(XMVECTOR v,TCHAR*name)
    {
    TCHAR buffer[1024]=_T("");
    _stprintf(buffer,_T("%s=( %.3f, %.3f)"),name,XMVectorGetX(v),XMVectorGetY(v));
    return buffer;
    }

inline TSTR VectorToString(XMVECTOR v,TCHAR*name)
    {
    TCHAR buffer[1024]=_T("");
    _stprintf(buffer,_T("%s=( %.3f, %.3f, %.3f )"),name,XMVectorGetX(v),XMVectorGetY(v),XMVectorGetZ(v));
    return buffer;
    }

inline TSTR Vector4ToString(XMVECTOR v,TCHAR*name)
    {
    TCHAR buffer[1024]=_T("");
    _stprintf(buffer,_T("%s=( %.3f, %.3f, %.3f, %.3f )"),name,XMVectorGetX(v),XMVectorGetY(v),XMVectorGetZ(v),XMVectorGetW(v));
    return buffer;
    }

inline TSTR MatrixToString(XMFLOAT4X4 mProj,TCHAR*name)
    {
    TCHAR buffer[1024]=_T("");
    _stprintf(buffer,_T("[ %.3f, %.3f, %.3f, %.3f\n%.3f, %.3f, %.3f, %.3f\n%.3f, %.3f, %.3f, %.3f\n%.3f, %.3f, %.3f, %.3f ] = %s"),
        mProj._11,mProj._12,mProj._13,mProj._14,
        mProj._21,mProj._22,mProj._23,mProj._24,
        mProj._31,mProj._32,mProj._33,mProj._34,
        mProj._41,mProj._42,mProj._43,mProj._44,name);
    return buffer;
    }

inline TSTR MatrixToString(XMMATRIX& m,TCHAR*name)
    {
    XMFLOAT4X4 mProj;
    XMStoreFloat4x4(&mProj,m);
    return MatrixToString(mProj,name);
    }

//round down to a power of 2 -----------------------------------------------------------------
inline NAT __cdecl CutDPow2(NAT num)
    {
    unsigned long msb;
    if(_BitScanReverse(&msb,num))
        return 1<<msb;
    return 1;
    }


struct ObjectMove
    {
    XMVECTOR A,B;
    double duration;

    
    ObjectMove(XMVECTOR A_,XMVECTOR B_,float duration_):A(A_),B(B_),duration(duration_)
        {
        }

    XMVECTOR LinearPosition(double t)
        {
        return A+(B-A)*(t/duration);
        }
    };


class Animation
    {
    public:
        std::vector<ObjectMove> moves;
        Ceas ceas;
        double period; //sec
        XMVECTOR B;
        bool finished;

    Animation():period(0.),finished(true)
        {
        }

    
    void Cancel()
        {
        ceas.mark=0;
        }


    void Reset()
        {
        moves.clear();
        finished=false;
        }


    bool Finished()
        {
        if(!moves.size())
            return true; // didn't even start
        if(GetAsyncKeyState(VK_SHIFT)&0x8000)
            {
            Cancel();
            return true;
            }
        bool new_finished=ceas.Sec()>period*moves.size();
        if(new_finished)
            {
            if(finished)
                return true;
            else
                finished=new_finished;
            }
        return false;
        }


    void Start1(XMVECTOR a,XMVECTOR b,float duration)
        {
        Reset();
        B=b;
        period=duration;
        moves.push_back(ObjectMove(a,b,period));
        ceas.Abs();
        }


    void Start2(XMVECTOR a,XMVECTOR b,float duration)
        {
        Reset();
        B=b;
        period=duration/2;
        moves.push_back(ObjectMove(a,b,period));
        moves.push_back(ObjectMove(b,a,period));
        ceas.Abs();
        }


    void Start3(XMVECTOR a,XMVECTOR b,XMVECTOR h,float duration)
        {
        Reset();
        B=b;
        period=duration/3;
        moves.push_back(ObjectMove(a,a+h,period));
        moves.push_back(ObjectMove(a+h,b+h,period));
        moves.push_back(ObjectMove(b+h,b,period));
        ceas.Abs();
        }


    bool LinearPosition(XMVECTOR&pos)
        {
        if(finished)
            {
            pos=B;
            return true;
            }
        double t=ceas.Sec();
        int m=t/period;
        if(m>=moves.size())
            {
            pos=B;
            return false;
            }
        pos=moves[m].LinearPosition(t-m*period);
        return true;
        }
    };
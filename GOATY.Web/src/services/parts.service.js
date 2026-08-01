import http from './http.js'

export function getParts(params = {}){
    return http.get('/api/parts', { params });
}

export function getPart(id){
    return http.get(`/api/parts/${id}`);
}

export function addPart(payload){
    return http.post('/api/parts', payload);
}

export function updatePart(id, payload){
    return http.put(`/api/parts/${id}`, payload);
}

export function deletePart(id){
    return http.delete(`/api/parts/${id}`);
}
